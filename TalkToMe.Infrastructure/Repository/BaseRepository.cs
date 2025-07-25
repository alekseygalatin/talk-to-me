﻿using Amazon.DynamoDBv2.DataModel;
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

        public async Task<List<T>> GetManyByIdAsync(string key)
        {
            return await _context.QueryAsync<T>(key).GetRemainingAsync();
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
            var entity = await _context.LoadAsync<T>(key);
            if (entity != null)
            {
                await _context.DeleteAsync(entity);
            }
        }
        
        public async Task DeleteManyAsync(string key)
        {
            var items = await GetManyByIdAsync(key);
            foreach (var item in items)
            {
                await _context.DeleteAsync(item);
            }
        }
    }
}
