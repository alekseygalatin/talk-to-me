using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository;

public class ChatHistoryRepository : BaseRepository<ChatHistoryEntity>, IChatHistoryRepository
{
    private readonly DynamoDBContext _context;

    public ChatHistoryRepository(IAmazonDynamoDB dynamoDb): base(dynamoDb) 
    {
        var config = new DynamoDBContextConfig
        {
            Conversion = DynamoDBEntryConversion.V2
        };
        _context = new DynamoDBContext(dynamoDb, config);
    }
    
    public async Task<IList<ChatHistoryEntity>> GetChatHistoryAsync(string id)
    {
        var query = _context.QueryAsync<ChatHistoryEntity>(id);
        return await query.GetRemainingAsync();
    }
}