using TalkToMe.Domain.Entities;

namespace TalkToMe.Infrastructure.IRepository
{
    public interface IWordRepository : IBaseRepository<WordEntity>
    {
        Task<List<WordEntity>> GetWordsByLanguageAsync(string userId, string language);
        Task<WordEntity?> GetWordAsync(string userId, string language, string word);
    }
}
