using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace GuestAccounts.Adapters.DynamoDb
{
    public class DbBuilder
    {
        private readonly IAmazonDynamoDB _db;

        public DbBuilder(IAmazonDynamoDB db)
        {
            _db = db;
        }
        public async Task Create()
        {
            using (AmazonDynamoDBClient client = new AmazonDynamoDBClient())
            {
                var tableStatus = await FindTables(client, new[] {"Accounts","Outbox"});
                await CreateMissingTables(client, tableStatus);
            }
        }

        private async Task CreateMissingTables(IAmazonDynamoDB client, Dictionary<string, bool> tableStatus)
        {
            if (tableStatus["Accounts"])
            {
                await CreateAccountsTable(client);
            }

            if (tableStatus["Outbox"])
            {
                await CreateOutboxTable(client);
            }
        }

       public async Task EnsureDb()
        {
            var client = new AmazonDynamoDBClient();      
            var status = "";

            do
            {
                // Wait 5 seconds before checking (again).
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                
                try
                {
                    var response = await client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = "Outbox"
                    });

                    Console.WriteLine("Table = {0}, Status = {1}", response.Table.TableName, response.Table.TableStatus);

                    status = response.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might get resource not found. 
                }

            } while (status != TableStatus.ACTIVE);
        }
       
        private async Task CreateAccountsTable(IAmazonDynamoDB client)
        {
            var request = new CreateTableRequest
            {
                TableName = "Accounts",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "Id", AttributeType = "N" },
                    new AttributeDefinition { AttributeName = "FirstName", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "LastName", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "FirstLineOfAddress", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "ZipCode", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "State", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "Email", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "TelephoneNumber ", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "CardNumber", AttributeType = "S" },
                    new AttributeDefinition {AttributeName = "CardSecurityCode", AttributeType = "S"} 
                    
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "Id", KeyType = "HASH"}
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            };

            var response = await client.CreateTableAsync(request);
            if (!response.HttpStatusCode.IsSuccessStatusCode())
                throw new ApplicationException(response.ToString());

        }

        private async Task CreateOutboxTable(IAmazonDynamoDB client)
        {
            var request = new CreateTableRequest
            {
                TableName = "Outbox",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "Id", AttributeType = "N" },
                    new AttributeDefinition { AttributeName = "Added", AttributeType = "S" },
                    new AttributeDefinition {AttributeName = "Dispatched", AttributeType = "S"} 
                    
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "Id", KeyType = "HASH"}
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            };

            var response = await client.CreateTableAsync(request);
            if (!response.HttpStatusCode.IsSuccessStatusCode())
                throw new ApplicationException(response.ToString());
        }
       
        private async Task<Dictionary<string, bool>> FindTables(IAmazonDynamoDB client, IEnumerable<string> tableNames)
        {
            var results = tableNames.ToDictionary(tableName => tableName, tableName => false);

            string lastEvaluatedTableName = null;
            do
            {
                var request = new ListTablesRequest
                {
                    Limit = 10, // Page size.
                    ExclusiveStartTableName = lastEvaluatedTableName
                };

                var response = await client.ListTablesAsync();
                var matches = response.TableNames.Intersect(tableNames);
                foreach (var match in matches)
                {
                    results[match] = true;
                }

                lastEvaluatedTableName = response.LastEvaluatedTableName;
            } while (lastEvaluatedTableName != null);

            return results;
        }
   }
}