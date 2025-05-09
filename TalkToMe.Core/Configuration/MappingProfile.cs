using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Core.Configuration
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<WordRequestDto, WordEntity>();
            CreateMap<WordEntity, WordResponseDto>();
            CreateMap<FeedbackRequestDto, Feedback>();
            CreateMap<SubscriptionRequestDto, Subscription>();
        }
    }
}
