using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Core.Configuration
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<UserPreference, UserPreferenceResponseDto>();
            CreateMap<UserPreferenceRequestDto, UserPreference>();
        }
    }
}
