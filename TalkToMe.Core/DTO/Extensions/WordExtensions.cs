using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Core.DTO.Extensions
{
    public static class WordExtensions
    {
        public static WordResponseDto ToResponseDto(this WordEntity entity) 
        {
            return new WordResponseDto
            {
                Transcription = entity.Transcription,
                Language = entity.Language,
                Word = entity.Word,
                Translations = entity.Translations,
                Example = entity.Example,
                IncludeIntoChat = entity.IncludeIntoChat
            };
        }

        public static WordEntity ToEntity(this WordRequestDto dto, string? userId = null) 
        {
            return new WordEntity
            {
                UserId = userId,
                LanguageWord = dto.LanguageWord,
                Transcription = dto.Transcription,
                Translations = dto.Translations,
                Example = dto.Example,
            };
        }

        public static List<WordResponseDto> ToResponseList(this IEnumerable<WordEntity> entityList) 
        {
            return entityList.Select(x => x.ToResponseDto()).ToList();
        }

        public static List<WordEntity> ToEntityList(this IEnumerable<WordRequestDto> dtos) 
        {
            return dtos.Select(x => x.ToEntity()).ToList();
        }

    }
}
