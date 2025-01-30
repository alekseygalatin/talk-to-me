using System.Globalization;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Enums;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    private IHistoryService _historyService;
    
    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }
        
    [HttpGet("{locale}/{agent}")]
    public async Task<ActionResult> Invoke([FromRoute] string locale, [FromRoute] string agent)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        if (agent == "conversationAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var history = await _historyService.GetHistory(sub + "6a47a518-cc4b-4b40-b7a5-94113d8c1b16");
                var result = history.Select(x => new HistoryMessageDto
                {
                    Message = x.Message,
                    IsUser = x.Role == ChatRole.User,
                    DateTime = DateTimeOffset.FromUnixTimeMilliseconds(x.Timestamp).UtcDateTime.ToString(CultureInfo.InvariantCulture)
                });
                return Ok(result);
            }
            else
            {
                var history = await _historyService.GetHistory(sub + "7b542e6e-7e2a-424f-8e76-0fec63cb2568");
                var result = history.Select(x => new HistoryMessageDto
                {
                    Message = x.Message,
                    IsUser = x.Role == ChatRole.User,
                    DateTime = DateTimeOffset.FromUnixTimeMilliseconds(x.Timestamp).UtcDateTime.ToString(CultureInfo.InvariantCulture)
                });
                return Ok(result);
            }
        } 
        else if (agent == "wordTeacherAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var history = await _historyService.GetHistory(sub + "f9b126e3-4340-4009-8b72-29a4ec321e7c");
                var result = history.Select(x => new HistoryMessageDto
                {
                    Message = x.Message,
                    IsUser = x.Role == ChatRole.User,
                    DateTime = DateTimeOffset.FromUnixTimeMilliseconds(x.Timestamp).UtcDateTime.ToString(CultureInfo.InvariantCulture)
                });
                return Ok(result);
            }
            else
            {
                var history = await _historyService.GetHistory(sub + "4e4f7fdd-ad93-4189-92af-d711c92aa751");
                var result = history.Select(x => new HistoryMessageDto
                {
                    Message = x.Message,
                    IsUser = x.Role == ChatRole.User,
                    DateTime = DateTimeOffset.FromUnixTimeMilliseconds(x.Timestamp).UtcDateTime.ToString(CultureInfo.InvariantCulture)
                });
                return Ok(result);
            }
        }
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
}