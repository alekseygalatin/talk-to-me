using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;

namespace TalkToMe.Core.Interfaces
{
    public interface ILanguageService
    {
        Task<List<LanguageResponseDto>> GetAllLanguagesAsync(bool onlyActive = true);
        Task<LanguageResponseDto> GetByIdAsync(string code);
        Task CreateAsync(LanguageRequestDto dto);
        Task UpdateAsync(LanguageRequestDto dto);
        Task DeleteAsync(string code);
    }
}
