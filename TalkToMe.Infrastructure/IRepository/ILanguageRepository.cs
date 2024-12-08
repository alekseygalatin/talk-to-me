using TalkToMe.Domain.Entities;

namespace TalkToMe.Infrastructure.IRepository
{
    public interface ILanguageRepository: IBaseRepository<Language>
    {
        Task<List<Language>> GetAllLanguagesAsync(bool onlyActive = true);
    }
}
