using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using NUnit.Framework;
using Paramore.Brighter.DynamoDb.Extensions;

namespace JustRoomsTests
{
    public abstract class DynamoDbBaseTest: IDisposable
    {
        
        private bool _disposed;
        private DynamoDbTableBuilder _dynamoDbTableBuilder;
        protected AWSCredentials Credentials { get; private set; }
        protected string TableName { get; private set; }
        public IAmazonDynamoDB Client { get; private set; }

        [SetUp]
        public void Initialize()
        {
            //required by AWS 2.2
            Environment.SetEnvironmentVariable("AWS_ENABLE_ENDPOINT_DISCOVERY", "false");
            
            Client = CreateClient();
            _dynamoDbTableBuilder = new DynamoDbTableBuilder(Client);
            
            
            //create a table request
            var createTableRequest = CreateTableRequest();
            TableName = createTableRequest.TableName;
            (bool exist, IEnumerable<string> tables) hasTables = _dynamoDbTableBuilder.HasTables(new string[] {TableName}).Result;
            if (!hasTables.exist)
            {
                var buildTable = _dynamoDbTableBuilder.Build(createTableRequest).Result;
                _dynamoDbTableBuilder.EnsureTablesReady(new[] {createTableRequest.TableName}, TableStatus.ACTIVE).Wait();
            }
        }

        /* 
        {
            YOU NEED TO IMPLEMENT SOMETHING SIMILAR TO BELOW. FOR YOUR DYNAMODB TABLE TYPE
            var createTableRequest = new DynamoDbTableFactory().GenerateCreateTableMapper<YOUR_TYPE_GOES_HERE>(
                new DynamoDbCreateProvisionedThroughput(
                    new ProvisionedThroughput {ReadCapacityUnits = 10, WriteCapacityUnits = 10},
                    new Dictionary<string, ProvisionedThroughput>()
                ));
            return createTableRequest;
        }
        */
        protected abstract CreateTableRequest CreateTableRequest();
        


        private IAmazonDynamoDB CreateClient()
        {
            Credentials = new BasicAWSCredentials("FakeAccessKey", "FakeSecretKey");

            var clientConfig = new AmazonDynamoDBConfig();
            clientConfig.ServiceURL = "http://localhost:8000";

            return new AmazonDynamoDBClient(Credentials, clientConfig);
 
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DynamoDbBaseTest()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            var tableNames = new string[] {TableName};
            //var deleteTables =_dynamoDbTableBuilder.Delete(tableNames).Result;
           // _dynamoDbTableBuilder.EnsureTablesDeleted(tableNames).Wait();
 
            _disposed = true;
       }
     }
}