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
    public class UserPreferencesController : ControllerBase
    {
        private readonly IUserPreferenceService _service;

        public UserPreferencesController(IUserPreferenceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            var preferences = await _service.GetByIdAsync(UserHelper.GetUserId(User));
            return Ok(preferences);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserPreferenceRequestDto dto)
        {
            var userId = UserHelper.GetUserId(User);
            await _service.CreateAsync(UserHelper.GetUserId(User), dto);
            return CreatedAtAction(nameof(GetById), new { userId }, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserPreferenceRequestDto dto)
        {
            try
            {
                await _service.UpdateAsync(UserHelper.GetUserId(User), dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await _service.DeleteAsync(UserHelper.GetUserId(User));
            return NoContent();
        }

        [HttpPut("current-language")]
        public async Task<IActionResult> SetCurrentLanguageToLearn([FromBody] LanguageInfoDTO languageInfo)
        {
            try
            {
                await _service.SetCurrentLanguageToLearn(UserHelper.GetUserId(User), languageInfo.LanguageCode);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }

}
