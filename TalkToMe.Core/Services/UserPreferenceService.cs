using AutoMapper;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services
{
    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly IBaseRepository<UserPreference> _repository;
        private readonly IMapper _mapper;

        public UserPreferencesService(IBaseRepository<UserPreference> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserPreferenceResponseDto> GetByIdAsync(string userId)
        {
            if(string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var preferences = await _repository.GetByIdAsync(userId);
            if (preferences == null) return null;

            return _mapper.Map<UserPreferenceResponseDto>(preferences);
        }

        public async Task CreateAsync(string userId, UserPreferenceRequestDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var userPreferecnce = _mapper.Map<UserPreference>(dto);
            userPreferecnce.UserId = userId;

            await _repository.CreateAsync(userPreferecnce);
        }

        public async Task UpdateAsync(string userId, UserPreferenceRequestDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var preferences = await _repository.GetByIdAsync(userId);
            if (preferences == null)
                throw new KeyNotFoundException("User preferences not found.");

            preferences.Name = dto.Name;
            preferences.Sex = dto.Sex;
            preferences.NativeLanguage = dto.NativeLanguage;

            await _repository.UpdateAsync(preferences);
        }

        public async Task DeleteAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var preferences = await _repository.GetByIdAsync(userId);
            if (preferences == null)
                throw new KeyNotFoundException("User preferences not found.");

            await _repository.DeleteAsync(userId);
        }
    }

}
