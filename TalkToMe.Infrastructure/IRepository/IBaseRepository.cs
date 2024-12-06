
namespace TalkToMe.Infrastructure.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string userId);
        Task CreateAsync(T preferences);
        Task UpdateAsync(T preferences);
        Task DeleteAsync(string userId);
    }
}
