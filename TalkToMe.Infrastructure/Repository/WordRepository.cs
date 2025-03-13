using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using TalkToMe.Domain.Constants;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository;

public class WordRepository : BaseRepository<WordEntity>, IWordRepository
{
    private readonly DynamoDBContext _context;
    private readonly IAmazonDynamoDB _dynamoDb;

    public WordRepository(IAmazonDynamoDB dynamoDb): base(dynamoDb) 
    {
        var config = new DynamoDBContextConfig
        {
            Conversion = DynamoDBEntryConversion.V2
        };
        _context = new DynamoDBContext(dynamoDb, config);
        _dynamoDb = dynamoDb;
    }

    public async Task<int> CountWordsByLanguageAsync(string userId, string language)
    {
        var request = new QueryRequest
        {
            TableName = TableNames.WordsTable,
            KeyConditionExpression = "UserId = :userId AND begins_with(LanguageWord, :languageWord)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":userId", new AttributeValue { S = userId } },
            { ":languageWord", new AttributeValue { S = $"{language}#" } }
        },
            Select = "COUNT" // Only return count, no data
        };

        var response = await _dynamoDb.QueryAsync(request);
        return response.Count;
    }


    public async Task<List<WordEntity>> GetWordsByLanguageAsync(string userId, string language)
    {
        var query = _context.QueryAsync<WordEntity>(
            userId,
            QueryOperator.BeginsWith,
            new[] { $"{language}#" });

        return await query.GetRemainingAsync();
    }

    public async Task<List<string>> GetRandomWordsAsync(string userId, string language, int count)
    {
        var scanRequest = new ScanRequest
        {
            TableName = TableNames.WordsTable,
            FilterExpression = "UserId = :userId AND begins_with(LanguageWord, :languageWord) AND IncludeIntoChat = :includeIntoChat",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":userId", new AttributeValue { S = userId } },
            { ":languageWord", new AttributeValue { S = $"{language}#" } },
            { ":includeIntoChat", new AttributeValue { BOOL = true } }
        },
            ProjectionExpression = "LanguageWord", // Select only LanguageWord field
            Limit = count * 5 // Fetch extra records for better randomness
        };

        var response = await _dynamoDb.ScanAsync(scanRequest);

        return response.Items
            .Select(item => item["LanguageWord"].S.Split('#')[1]) // Extract word after language#
            .OrderBy(_ => Guid.NewGuid()) // Shuffle
            .Take(count) // Select required count
            .ToList();
    }

    public async Task<WordEntity?> GetWordAsync(string userId, string language, string word)
    {
        var query = _context.QueryAsync<WordEntity>(
            userId,
            QueryOperator.Equal,
            new[] { $"{language}#{word}" });

        return (await query.GetRemainingAsync()).FirstOrDefault();
    }

    public async Task DeleteAsync(WordEntity word)
    {
        await _context.DeleteAsync(word);
    }

    public async Task UpdateIncludeIntoChatAsync(string userId, string languageWord, bool includeIntoChat)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(languageWord))
            throw new ArgumentException("UserId and LanguageWord cannot be null or empty.");

        var updateRequest = new UpdateItemRequest
        {
            TableName = TableNames.WordsTable,
            Key = new Dictionary<string, AttributeValue>
            {
                { "UserId", new AttributeValue { S = userId } },
                { "LanguageWord", new AttributeValue { S = languageWord } }
            },
            UpdateExpression = "SET IncludeIntoChat = :includeIntoChat",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":includeIntoChat", new AttributeValue { BOOL = includeIntoChat } }
            }
        };

        await _dynamoDb.UpdateItemAsync(updateRequest);
    }
}