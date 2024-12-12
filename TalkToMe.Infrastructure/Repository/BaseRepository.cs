using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository
{
    public class BaseRepository<T>: IBaseRepository<T> where T : class
    {
        private readonly DynamoDBContext _context;

        public BaseRepository(IAmazonDynamoDB dynamoDb)
        {
            var config = new DynamoDBContextConfig
            {
                Conversion = DynamoDBEntryConversion.V2
            };
            _context = new DynamoDBContext(dynamoDb, config);
        }

        public async Task<List<T>> GetAllAsync()
        {
            var conditions = new List<ScanCondition>(); // No filter, retrieves all items
            return await _context.ScanAsync<T>(conditions).GetRemainingAsync();
        }

        public async Task<T> GetByIdAsync(string key)
        {
            return await _context.LoadAsync<T>(key);
        }

        public async Task CreateAsync(T entity)
        {
            await _context.SaveAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await _context.SaveAsync(entity);
        }

        public async Task DeleteAsync(string key)
        {
            await _context.DeleteAsync<T>(key);
        }
    }
}
