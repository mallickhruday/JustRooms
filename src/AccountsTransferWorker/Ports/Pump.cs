using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Polly;
using Polly.Registry;

namespace AccountsTransferWorker.Ports
{
    public class Pump : BackgroundService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IAmazonDynamoDBStreams _dynamoDBStream;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;
        private readonly ILogger<Pump> _logger;
        private string _latestStreamArn;
        private string _latestStreamLabel;
        private const int maxItemCount = 100;

        public Pump(
            IAmazonDynamoDB dynamoDb,
            IAmazonDynamoDBStreams dynamoDbStream, 
            IAmACommandProcessor commandProcessor, 
            IReadOnlyPolicyRegistry<string> policyRegistry,
            ILogger<Pump> logger = null)
        {
            _dynamoDb = dynamoDb;
            _dynamoDBStream = dynamoDbStream;
            _commandProcessor = commandProcessor;
            _policyRegistry = policyRegistry;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var table = await _dynamoDb.DescribeTableAsync(Globals.AccountsTableName, cancellationToken);
            _latestStreamArn = table.Table.LatestStreamArn;
            _latestStreamLabel = table.Table.LatestStreamLabel;
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var policyWrap = Policy.WrapAsync(
                _policyRegistry.Get<IAsyncPolicy>(CommandProcessor.CIRCUITBREAKERASYNC),
                _policyRegistry.Get<IAsyncPolicy>(CommandProcessor.RETRYPOLICYASYNC)
                );

            await policyWrap.ExecuteAsync(async () => 
            {
                string lastShardId = null;
                do 
                {
                    var describeStreamResult = await _dynamoDBStream.DescribeStreamAsync(
                        new DescribeStreamRequest
                            {
                                StreamArn = _latestStreamArn,
                                ExclusiveStartShardId = lastShardId
                            }
                        );
                    
                    // Process each shard on this page
                    foreach (var shard in describeStreamResult.StreamDescription.Shards) 
                    {
                        var shardId = shard.ShardId;
        
                        // Get an iterator for the current shard
                        var iteratorRequest = new GetShardIteratorRequest 
                        {
                            StreamArn =_latestStreamArn,
                            ShardId = shard.ShardId,
                            ShardIteratorType = ShardIteratorType.TRIM_HORIZON
                        };
                        
                        var iteratorResult = await _dynamoDBStream.GetShardIteratorAsync(iteratorRequest,stoppingToken);
                        var iterator = iteratorResult.ShardIterator;
        
                        // Shard iterator is not null until the Shard is sealed (marked as READ_ONLY).
                        // To prevent running the loop until the Shard is sealed, which will be on average
                        // 4 hours, we process only the items that were written into DynamoDB and then exit.
                        var processedRecordCount = 0;
                        while (iterator != null && processedRecordCount < maxItemCount) 
                        {
                            // Use the shard iterator to read the stream records
                            var recordsResult = await _dynamoDBStream.GetRecordsAsync(
                                new GetRecordsRequest
                                {
                                    ShardIterator = iterator
                                });
                            
                            var records = recordsResult.Records;
                            foreach(var record in records) 
                            {
                                _logger.LogDebug(record.Dynamodb.SequenceNumber);
                            }
                            iterator = recordsResult.NextShardIterator;
                        }
                    }
        
                    // If lastShardId is set, then there is at least one more page to retrieve
                    lastShardId = describeStreamResult.StreamDescription.LastEvaluatedShardId;
                } while (lastShardId != null);
            });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping pump");
            await base.StopAsync(cancellationToken);
        }
    }
}