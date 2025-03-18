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

        [HttpPost]
        public async Task<IActionResult> StartSession([FromBody] LanguageInfoDto request)
        {
            var words = await _vocabularyChatSessionStore.CreateSession(UserHelper.GetUserId(User), request.LanguageCode, 10);

            return Ok(words);
        }

        [HttpGet("{languageCode}/words")]
        public List<string> GetWords(string languageCode)
        {
            var words = _vocabularyChatSessionStore.GetWords(UserHelper.GetUserId(User), languageCode);

            return words;
        }

        [HttpDelete("{languageCode}")]
        public IActionResult EndSession(string languageCode)
        {
            _vocabularyChatSessionStore.RemoveSession(UserHelper.GetUserId(User), languageCode);
            return NoContent();
        }

    }
}
