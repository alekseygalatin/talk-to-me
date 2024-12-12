using AutoMapper;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _repository;
        private readonly IMapper _mapper;

        public LanguageService(ILanguageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<LanguageResponseDto>> GetAllLanguagesAsync(bool onlyActive = true)
        {
            var languages = await _repository.GetAllLanguagesAsync();
            return _mapper.Map<List<LanguageResponseDto>>(languages);
        }

        public async Task<LanguageResponseDto> GetByIdAsync(string code)
        {
            if(string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            var language = await _repository.GetByIdAsync(code);
            if (language == null) return null;

            return _mapper.Map<LanguageResponseDto>(language);
        }

        public async Task CreateAsync(LanguageRequestDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var language = _mapper.Map<Language>(dto);

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
