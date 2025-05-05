using TalkToMe.Core.DTO.Extensions;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Interfaces;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _repository;

        public LanguageService(ILanguageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<LanguageResponseDto>> GetAllLanguagesAsync(bool onlyActive = true)
        {
            var languages = await _repository.GetAllLanguagesAsync();
            return languages.ToResponseList();
        }

        public async Task<LanguageResponseDto> GetByIdAsync(string code)
        {
            if(string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            var language = await _repository.GetByIdAsync(code);
            if (language == null) return null;

            return language.ToResponseDto();
        }

        public async Task CreateAsync(LanguageRequestDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var language = dto.ToEntity();

            await _repository.CreateAsync(language);
        }

        public async Task UpdateAsync(LanguageRequestDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var language = await _repository.GetByIdAsync(dto.Code);
            if (language == null)
                throw new KeyNotFoundException("Language not found.");

            language.Name = dto.Name;
            language.EnglishName = dto.EnglishName;
            language.Active = dto.Active;
            language.Pronouns = dto.Pronouns;

            await _repository.UpdateAsync(language);
        }

        public async Task DeleteAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            var language = await _repository.GetByIdAsync(code);
            if (language == null)
                throw new KeyNotFoundException("Language not found.");

            await _repository.DeleteAsync(code);
        }
    }

}
