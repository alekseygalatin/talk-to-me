﻿using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;

namespace TalkToMe.Core.Interfaces
{
    public interface IUserPreferenceService
    {
        Task<UserPreferenceResponseDto> GetByIdAsync(string userId);
        Task CreateAsync(string userId, UserPreferenceRequestDto dto);
        Task UpdateAsync(string userId, UserPreferenceRequestDto dto);
        Task DeleteAsync(string userId);
        Task SetCurrentLanguageToLearn(string userId, string languageCode);
    }
}
