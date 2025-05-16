using TalkToMe.Core.DTO.Request;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Core.DTO.Extensions
{
    public static class SubscriptionExtensions
    {
        public static Subscription ToEntity(this SubscriptionRequestDto dto) 
        {
            return new Subscription
            {
                UserId = dto.UserId,
                Comment = dto.Comment,
                CreatedAt = dto.CreatedAt != null ? dto.CreatedAt.Value : 0
            };
        }

        public static List<Subscription> ToEntityList(this IEnumerable<SubscriptionRequestDto> dtos) 
        {
            return dtos.Select(x => x.ToEntity()).ToList();
        }

    }
}
