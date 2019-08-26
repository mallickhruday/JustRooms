using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountsTransferWorker.Ports;
using AccountsTransferWorker.Ports.Events;
using AccountsTransferWorker.Ports.Streams;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;

namespace AccountsTransferWorker.Adapters.Data
{
    public class DynamoStreamReader : IStreamReader
    {
        public const int BatchSize = 1000;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IAmazonDynamoDBStreams _dynamoDbStreams;
        private readonly IRecordProcessor<StreamRecord> _recordProcessor;
        private readonly ILogger<Pump> _logger;
        private string _latestStreamArn;

        public DynamoStreamReader(
            IAmazonDynamoDB dynamoDb,
            IAmazonDynamoDBStreams dynamoDbStreams,
            IRecordProcessor<StreamRecord> recordProcessor,
            ILogger<Pump> logger = null)
        {
            _dynamoDb = dynamoDb;
            _dynamoDbStreams = dynamoDbStreams;
            _recordProcessor = recordProcessor;
            _logger = logger;
        }

        public async Task InitialiseReader(CancellationToken cancellationToken)
        {
             var table = await _dynamoDb.DescribeTableAsync(Globals.AccountsTableName, cancellationToken);
            _latestStreamArn = table.Table.LatestStreamArn;
        }

        public async Task ReadStream(CancellationToken stoppingToken)
        {
            //TODO; We don't track the shards that we have read yet. We need to, as we don't want to process
            //multiple times.
            //TODO: We don't cope with a shards being split or joined. To do that we would need to track parent shards
            // and ensure that we read the adjacent parent following a re-shard.
            
            var shards = await FindShards();
            var maxParallelism = shards.Count;
            await shards.Values.ForEachAsync(maxParallelism, shard => ProcessShard(stoppingToken, shard));
        }
        
        private async Task<Dictionary<string, Shard>> FindShards()
        {
            var shards = new Dictionary<string, Shard>();
            string lastShardId = null;
            do
            {
                var describeStreamResult = await _dynamoDbStreams.DescribeStreamAsync(
                    new DescribeStreamRequest
                    {
                        StreamArn = _latestStreamArn,
                        ExclusiveStartShardId = lastShardId,
                        Limit = 25
                    }
                );
                var candidateShards = describeStreamResult.StreamDescription.Shards
                    .Select(shard => new KeyValuePair<string,Shard>(shard.ShardId, shard))
                    .ToList();

                foreach (var candidateShard in candidateShards)
                {
                    shards.TryAdd(candidateShard.Key, candidateShard.Value);
                }

                // If lastShardId is set, then there is at least one more page to retrieve
                lastShardId = describeStreamResult.StreamDescription.LastEvaluatedShardId;

            } while (lastShardId != null);
            
            return shards;
        }
        
        private async Task ProcessShard(CancellationToken stoppingToken, Shard shard)
        {
            // Get an iterator for the current shard
            var iteratorRequest = new GetShardIteratorRequest
            {
                StreamArn = _latestStreamArn,
                ShardId = shard.ShardId,
                ShardIteratorType = ShardIteratorType.TRIM_HORIZON
            };

            var iteratorResult = await _dynamoDbStreams.GetShardIteratorAsync(iteratorRequest, stoppingToken);
            var iterator = iteratorResult.ShardIterator;

            // Shard iterator is not null until the Shard is sealed (marked as READ_ONLY).
            while (iterator != null)
            {
                // Use the shard iterator to read the stream records
                var recordsResult = await _dynamoDbStreams.GetRecordsAsync(
                    new GetRecordsRequest
                    {
                        ShardIterator = iterator,
                        Limit = BatchSize
                    });

                var records = recordsResult.Records;
                foreach (var record in records)
                {
                    _recordProcessor.ProcessRecord(record.Dynamodb);
                }

                iterator = recordsResult.NextShardIterator;
                
                //we need to pause between iterations as get records has a limit
                await Task.Delay(1000);

            }

            throw new ShardHasBeenClosedException(
                $"The shard has been closed and we must re-get shards. Shard {shard.ShardId}"
                );
        }


    }
}