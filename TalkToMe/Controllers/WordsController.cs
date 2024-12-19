using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.Agents;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class WordsController : ControllerBase
{
    private SwedishWordTeacherAgent _swedishWordTeacher;
    private IWordService _wordService;
    
    public WordsController(IWordService wordService)
    {
        _wordService = wordService;
    }

    [HttpPost("invoke")]
    public async Task<IActionResult> GetResponse([FromBody] string text)
    {
        var sub = "123";//this.HttpContext.User.Claims.First(x => x.Type.Equals("sub")).Value;
        var result = await _swedishWordTeacher.Invoke(text, sub);
        return Ok(result.Response);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddWordToDictionary([FromBody] AddWordToDictionaryRequestDto request)
    {
        var sub = "123";//this.HttpContext.User.Claims.First(x => x.Type.Equals("sub")).Value;
        await _wordService.AddWordToDictionary(sub, request);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetWords()
    {
        var sub = "123";//this.HttpContext.User.Claims.First(x => x.Type.Equals("sub")).Value;
        var result = await _wordService.GetWords(sub);
        return Ok(result);
    }
}