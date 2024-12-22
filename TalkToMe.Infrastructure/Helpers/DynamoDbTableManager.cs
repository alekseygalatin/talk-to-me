using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Infrastructure.Helpers
{
    public class DynamoDbTableManager
    {
        private readonly IAmazonDynamoDB _dynamoDb;

        public DynamoDbTableManager(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task CreateTablesIfNotExist() 
        {
            var existingTables = await _dynamoDb.ListTablesAsync();

            if (!existingTables.TableNames.Contains(TableNames.UserPreferences))
                await CreateUserPreferencesTable();

            if (!existingTables.TableNames.Contains(TableNames.Languages)) 
            {
                await CreateLanguagesTable();
                await SeedLanguageDataAsync();
            }
        }

        private async Task CreateUserPreferencesTable() 
        {
            Console.WriteLine($"Creating table: {TableNames.UserPreferences}");
            var request = new CreateTableRequest
            {
                TableName = TableNames.UserPreferences,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "UserId",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "UserId",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            await _dynamoDb.CreateTableAsync(request);
            Console.WriteLine($"Table {TableNames.UserPreferences} created.");
        }

        private async Task CreateLanguagesTable()
        {
            Console.WriteLine($"Creating table: {TableNames.Languages}");
            var request = new CreateTableRequest
            {
                TableName = TableNames.Languages,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Code",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Code",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            await _dynamoDb.CreateTableAsync(request);
            Console.WriteLine($"Table {TableNames.Languages} created.");
        }

        public async Task SeedLanguageDataAsync()
        {
            await WaitForTableToBeActiveAsync(TableNames.Languages);

            if (!await TableContainsRecordsAsync(TableNames.Languages)) 
            {
                Console.WriteLine("Seed languages started");

                var languages = new List<PutItemRequest>
                {
                    new PutItemRequest
                    {
                        TableName = TableNames.Languages,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "Code", new AttributeValue { S = "en-US" } },
                            { "Name", new AttributeValue { S = "English" } },
                            { "EnglishName", new AttributeValue { S = "English" } },
                            { "Active", new AttributeValue { BOOL = true } },
                            { "Pronouns",  new AttributeValue { SS = new List<string>{ "He/Him", "She/Her", "They/Them" } } }
                        }
                    },
                    new PutItemRequest
                    {
                        TableName = TableNames.Languages,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "Code", new AttributeValue { S = "sv-SE" } },
                            { "Name", new AttributeValue { S = "Svenska" } },
                            { "EnglishName", new AttributeValue { S = "Swedish" } },
                            { "Active", new AttributeValue { BOOL = true } },
                            { "Pronouns",  new AttributeValue { SS = new List<string>{ "Han", "Hon", "Hen" } } }
                        }
                    },
                    new PutItemRequest
                    {
                        TableName = TableNames.Languages,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "Code", new AttributeValue { S = "ru" } },
                            { "Name", new AttributeValue { S = "Русский" } },
                            { "EnglishName", new AttributeValue { S = "Russian" } },
                            { "Active", new AttributeValue { BOOL = true } },
                            { "Pronouns",  new AttributeValue { SS = new List<string>{ "Он", "Она", "Оно" } } }
                        }
                    },
                    new PutItemRequest
                    {
                        TableName = TableNames.Languages,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "Code", new AttributeValue { S = "ky" } },
                            { "Name", new AttributeValue { S = "Кыргыз тили" } },
                            { "EnglishName", new AttributeValue { S = "Kyrgyz" } },
                            { "Active", new AttributeValue { BOOL = true } },
                            { "Pronouns",  new AttributeValue { SS = new List<string>{ "Ал" } } }
                        }
                    }
                };

                foreach (var request in languages)
                {
                    await _dynamoDb.PutItemAsync(request);
                }
                Console.WriteLine("Seed languages data inserted.");
            }
        }

        private async Task WaitForTableToBeActiveAsync(string tableName)
        {
            Console.WriteLine($"Waiting for table {tableName} to become active...");
            string tableStatus;
            do
            {
                var response = await _dynamoDb.DescribeTableAsync(new DescribeTableRequest { TableName = tableName });
                tableStatus = response.Table.TableStatus;
                if (tableStatus != "ACTIVE")
                {
                    await Task.Delay(5000); // Wait 5 seconds before checking again
                }
            } while (tableStatus != "ACTIVE");

            Console.WriteLine($"Table {tableName} is now active.");
        }

        public async Task<bool> TableContainsRecordsAsync(string tableName)
        {
            var request = new ScanRequest
            {
                TableName = tableName,
                Limit = 1 // Fetch only one item to check for records
            };

            var response = await _dynamoDb.ScanAsync(request);

            // If response has any items, the table contains records
            return response.Count > 0;
        }
    }
}
