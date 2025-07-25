﻿using TalkToMe.Core.DTO.Extensions;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly IBaseRepository<UserPreference> _repository;

        public UserPreferenceService(IBaseRepository<UserPreference> repository)
        {
            _repository = repository;
        }

        public async Task<UserPreferenceResponseDto> GetByIdAsync(string userId)
        {
            if(string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var preferences = await _repository.GetByIdAsync(userId);
            if (preferences == null) return null;

            return preferences.ToResponseDto();
        }

        public async Task CreateAsync(string userId, UserPreferenceRequestDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var userPreferecnce = dto.ToEntity(userId);

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
            preferences.PreferedPronoun = dto.PreferedPronoun;
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

        public async Task SetCurrentLanguageToLearn(string userId, string languageCode)
        {
            var userPreferences = await _repository.GetByIdAsync(userId);
            if (userPreferences == null)
                throw new Exception("User preferences not found");

            userPreferences.CurrentLanguageToLearn = languageCode;
            await _repository.UpdateAsync(userPreferences);
        }
    }

}
