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

        var wordEntity = await _context.LoadAsync<WordEntity>(userId, languageWord);
        if (wordEntity == null)
            throw new KeyNotFoundException("Word not found");

        wordEntity.IncludeIntoChat = includeIntoChat;

        await _context.SaveAsync(wordEntity);
    }
}