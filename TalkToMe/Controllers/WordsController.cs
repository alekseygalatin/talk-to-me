using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Interfaces;
using TalkToMe.Helpers;

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
    public async Task<IActionResult> AddWordToDictionary([FromBody] WordRequestDto request)
    {
        await _wordService.AddWordToDictionary(UserHelper.GetUserId(User), request);
        return NoContent();
    }

    [HttpGet("{language}")]
    public async Task<IActionResult> GetWords(string language)
    {
        var result = await _wordService.GetWords(UserHelper.GetUserId(User), language);
        return Ok(result);
    }

    [HttpDelete("{language}/{word}")]
    public async Task<IActionResult> DeleteWord(string language, string word)
    {
        await _wordService.DeleteWord(UserHelper.GetUserId(User), language, word);
        return Ok();
    }

    [HttpPut("{language}/chat/{word}")]
    public async Task<IActionResult> SetIncludeIntoChat(string language, string word, [FromQuery] bool include)
    {
        await _wordService.SetIncludeIntoChat(UserHelper.GetUserId(User), language, word, include);
        return NoContent();
    }
}