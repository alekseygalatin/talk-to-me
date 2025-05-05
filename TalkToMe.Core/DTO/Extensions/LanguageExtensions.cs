using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Core.DTO.Extensions
{
    public static class LanguageExtensions
    {
        public static LanguageResponseDto ToResponseDto(this Language entity) 
        {
            return new LanguageResponseDto
            {
                Code = entity.Code,
                Name = entity.Name,
                EnglishName = entity.EnglishName,
                Pronouns = entity.Pronouns
            };
        }

        public static Language ToEntity(this LanguageRequestDto dto) 
        {
            return new Language
            {
                Code = dto.Code,
                Name = dto.Name,
                EnglishName = dto.EnglishName,
                Active = dto.Active,
                Pronouns = dto.Pronouns
            };
        }

        public static List<LanguageResponseDto> ToResponseList(this IEnumerable<Language> entityList) 
        {
            return entityList.Select(x => x.ToResponseDto()).ToList();
        }

        public static List<Language> ToEntityList(this IEnumerable<LanguageRequestDto> dtos) 
        {
            return dtos.Select(x => x.ToEntity()).ToList();
        }

    }
}
