using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Interfaces;
using TalkToMe.Helpers;

namespace TalkToMe.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public async Task<bool> SubscriptionRequested()
        {
            return await _subscriptionService.SubscriptionRequested(UserHelper.GetUserId(User));
        }

        [HttpPost]
        public async Task<IActionResult> Subscription([FromBody] SubscriptionRequestDto subscription)
        {
            subscription.UserId = UserHelper.GetUserId(User);

            await _subscriptionService.RequestSubscription(subscription);
            return NoContent();
        }

        [HttpDelete]
        public async Task CancelSubscription()
        {
            await _subscriptionService.CancelSubscriptionRequest(UserHelper.GetUserId(User));
        }
    }
}
