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
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Feedback([FromBody] FeedbackRequestDto feedback)
        {
            feedback.UserId = UserHelper.GetUserId(User);

            await _feedbackService.SaveFeedbackAsync(feedback);
            return NoContent();
        }
    }
}
