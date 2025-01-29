using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Interfaces;

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
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
            var preferences = await _service.GetByIdAsync(userId);
            return Ok(preferences);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserPreferenceRequestDto dto)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
            await _service.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { userId }, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserPreferenceRequestDto dto)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;

            try
            {
                await _service.UpdateAsync(userId, dto);
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
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;

            await _service.DeleteAsync(userId);
            return NoContent();
        }

        [HttpPut("set-current-language-to-learn/{languageCode}")]
        public async Task<IActionResult> SetCurrentLanguageToLearn(string languageCode)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;

            try
            {
                await _service.SetCurrentLanguageToLearn(userId, languageCode);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }

}
