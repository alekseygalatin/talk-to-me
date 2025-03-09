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
    public class VocabularyChatSessionsController : ControllerBase
    {
        private IVocabularyChatSessionStore _vocabularyChatSessionStore;

        public VocabularyChatSessionsController(IVocabularyChatSessionStore vocabularyChatSessionStore)
        {
            _vocabularyChatSessionStore = vocabularyChatSessionStore;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] LanguageInfoDto request)
        {
            var words = await _vocabularyChatSessionStore.CreateSession(UserHelper.GetUserId(User), request.LanguageCode, 10);

            return Ok(words);
        }


        [HttpPost("end")]
        public IActionResult EndSession([FromBody] LanguageInfoDto request)
        {
            _vocabularyChatSessionStore.RemoveSession(UserHelper.GetUserId(User), request.LanguageCode);
            return Ok("Session ended");
        }

    }
}
