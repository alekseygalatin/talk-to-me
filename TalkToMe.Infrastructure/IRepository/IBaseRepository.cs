
namespace TalkToMe.Infrastructure.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string key);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string key);
        Task DeleteManyAsync(string key);
        Task<List<T>> GetManyByIdAsync(string key);
    }
}
