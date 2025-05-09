using TalkToMe.Core.DTO.Request;

namespace TalkToMe.Core.Interfaces
{
    public interface ISubscriptionService
    {
        Task RequestSubscription(SubscriptionRequestDto subscription);
        Task<bool> SubscriptionRequested(string userId);
        Task CancelSubscriptionRequest(string userId);
    }
}
