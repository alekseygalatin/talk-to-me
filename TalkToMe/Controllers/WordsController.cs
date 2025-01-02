using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WordsController : ControllerBase
{
    private IWordService _wordService;

    public WordsController(IWordService wordService)
    {
        _wordService = wordService;
    }

    [HttpPost]
    public async Task<IActionResult> AddWordToDictionary([FromBody] AddWordToDictionaryRequestDto request)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        await _wordService.AddWordToDictionary(sub, request);
        return NoContent();
    }

    [HttpGet("{language}")]
    public async Task<IActionResult> GetWords(string language)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        var result = await _wordService.GetWords(sub, language);
        return Ok(result);
    }
}