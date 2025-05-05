using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Core.DTO.Extensions
{
    public static class UserPreferenceExtensions
    {
        public static UserPreferenceResponseDto ToResponseDto(this UserPreference entity)
        {
            return new UserPreferenceResponseDto
            {
                UserId = entity.UserId,
                Name = entity.Name,
                NativeLanguage = entity.NativeLanguage,
                PreferedPronoun = entity.PreferedPronoun,
                CurrentLanguageToLearn = entity.CurrentLanguageToLearn
            };
        }

        public static UserPreference ToEntity(this UserPreferenceRequestDto dto, string? userId = null)
        {
            return new UserPreference
            {
                UserId = userId,
                Name = dto.Name,
                NativeLanguage = dto.NativeLanguage,
                PreferedPronoun = dto.PreferedPronoun,
                CurrentLanguageToLearn = dto.CurrentLanguageToLearn ?? string.Empty
            };
        }

        public static List<UserPreferenceResponseDto> ToResponseDtoList(this IEnumerable<UserPreference> entities)
        {
            return entities.Select(e => e.ToResponseDto()).ToList();
        }

        public static List<UserPreference> ToEntityList(this IEnumerable<UserPreferenceRequestDto> dtos)
        {
            return dtos.Select(dto => dto.ToEntity()).ToList();
        }
    }
}
