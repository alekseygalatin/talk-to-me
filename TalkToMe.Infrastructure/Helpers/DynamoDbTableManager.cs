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
                    }/*,
                    new AttributeDefinition
                    {
                        AttributeName = "Name",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Sex",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "NativeLanguage",
                        AttributeType = "S"
                    }*/
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
                    }/*,
                    new AttributeDefinition
                    {
                        AttributeName = "Name",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "EnglishName",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Active",
                        AttributeType = "BOOL"
                    }*/
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

        private async Task SeedLanguageDataAsync()
        {
            var english = new PutItemRequest
            {
                TableName = TableNames.Languages,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Code", new AttributeValue { S = "en-us" } },
                    { "Name", new AttributeValue { S = "English" } },
                    { "EnglishName", new AttributeValue { S = "English" } },
                    { "Active", new AttributeValue { BOOL = true } }
                }
            };

            await _dynamoDb.PutItemAsync(english);

            var swedish = new PutItemRequest
            {
                TableName = TableNames.Languages,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Code", new AttributeValue { S = "sv-se" } },
                    { "Name", new AttributeValue { S = "Svenska" } },
                    { "EnglishName", new AttributeValue { S = "Swedish" } },
                    { "Active", new AttributeValue { BOOL = true } }
                }
            };
            await _dynamoDb.PutItemAsync(swedish);

            var russian = new PutItemRequest
            {
                TableName = TableNames.Languages,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Code", new AttributeValue { S = "ru" } },
                    { "Name", new AttributeValue { S = "Русский " } },
                    { "EnglishName", new AttributeValue { S = "Russian" } },
                    { "Active", new AttributeValue { BOOL = true } }
                }
            };
            await _dynamoDb.PutItemAsync(russian);

            Console.WriteLine("Seed languages data inserted.");
        }
    }
}
