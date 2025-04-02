using TalkToMe.Core.DTO.Request;

namespace TalkToMe.Core.Interfaces
{
    public interface IFeedbackService
    {
        Task SaveFeedbackAsync(FeedbackRequestDto feedback);
    }
}
