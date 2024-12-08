using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IUserPreferenceService _service;

        public UserPreferencesController(IUserPreferenceService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(string userId)
        {
            var preferences = await _service.GetByIdAsync(userId);
            if (preferences == null) return NotFound();

            return Ok(preferences);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Create(string userId, [FromBody] UserPreferenceRequestDto dto)
        {
            await _service.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { userId }, null);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, [FromBody] UserPreferenceRequestDto dto)
        {
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

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            await _service.DeleteAsync(userId);
            return NoContent();
        }
    }

}
