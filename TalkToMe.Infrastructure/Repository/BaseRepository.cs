using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository
{
    public class BaseRepository<T>: IBaseRepository<T> where T : class
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly DynamoDBContext _context;

        public BaseRepository(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
            _context = new DynamoDBContext(dynamoDb);
        }

        public async Task<T> GetByIdAsync(string userId)
        {
            return await _context.LoadAsync<T>(userId);
        }

        public async Task CreateAsync(T preferences)
        {
            await _context.SaveAsync(preferences);
        }

        public async Task UpdateAsync(T preferences)
        {
            await _context.SaveAsync(preferences);
        }

        public async Task DeleteAsync(string userId)
        {
            await _context.DeleteAsync<UserPreference>(userId);
        }
    }
}
