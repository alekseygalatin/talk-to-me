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

    public async Task<List<WordEntity>> GetManyByIdAsync(string partitionKey, string sortKeyValue)
    {
        var query = _context.QueryAsync<WordEntity>(
            partitionKey,
            QueryOperator.Equal,
            new List<object> { sortKeyValue }
        );

        return await query.GetRemainingAsync();
    }
}