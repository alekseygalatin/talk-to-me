using TalkToMe.Core.DTO.Request;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Core.DTO.Extensions
{
    public static class FeedbackExtensions
    {
        public static Feedback ToEntity(this FeedbackRequestDto dto) 
        {
            return new Feedback
            {
                UserId = dto.UserId,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt != null ? dto.CreatedAt.Value : 0
            };
        }

        public static List<Feedback> ToEntityList(this IEnumerable<FeedbackRequestDto> dtos) 
        {
            return dtos.Select(x => x.ToEntity()).ToList();
        }

    }
}
