using TalkToMe.Domain.Entities;

namespace TalkToMe.Infrastructure.IRepository
{
    public interface IWordRepository : IBaseRepository<WordEntity>
    {
        Task<List<WordEntity>> GetManyByIdAsync(string partitionKey, string sortKeyValue);
    }
}
