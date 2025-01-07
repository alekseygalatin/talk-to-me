using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository;

public class WordRepository : BaseRepository<WordEntity>, IWordRepository
{
    private readonly DynamoDBContext _context;

    public WordRepository(IAmazonDynamoDB dynamoDb): base(dynamoDb) 
    {
        var config = new DynamoDBContextConfig
        {
            Conversion = DynamoDBEntryConversion.V2
        };
        _context = new DynamoDBContext(dynamoDb, config);
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
}