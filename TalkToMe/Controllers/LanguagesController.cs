using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _service;

        public LanguagesController(ILanguageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(bool onlyActive = true)
        {
            var languages = await _service.GetAllLanguagesAsync(onlyActive);
            if (languages.Count == 0) return NoContent();

            return Ok(languages);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetById(string code)
        {
            var language = await _service.GetByIdAsync(code);
            if (language == null) return NotFound();

            return Ok(language);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LanguageRequestDto dto)
        {
            await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { dto.Code }, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LanguageRequestDto dto)
        {
            try
            {
                await _service.UpdateAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            await _service.DeleteAsync(code);
            return NoContent();
        }
    }

}
