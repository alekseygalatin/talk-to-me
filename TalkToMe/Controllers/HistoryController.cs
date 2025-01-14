using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Services;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    private HistoryService _historyService;
    
    public HistoryController()
    {
        _historyService = new HistoryService();
    }
        
    [HttpGet("{locale}/{agent}")]
    public async Task<ActionResult> Invoke([FromRoute] string locale, [FromRoute] string agent)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        if (agent == "conversationAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var history = await _historyService.GetHistory(sub + "4");
                var result = history.Select(x => new HistoryMessageDto
                {
                    Message = x.Dialog.First().Message,
                    IsUser = x.Dialog.First().Role != "model",
                    DateTime = x.TimeStamp.ToString()
                });
                return Ok(result);
            }
            else
            {
                var history = await _historyService.GetHistory(sub + "1");
                var result = history.Select(x => new HistoryMessageDto
                {
                    Message = x.Dialog.First().Message,
                    IsUser = x.Dialog.First().Role != "model",
                    DateTime = x.TimeStamp.ToString()
                });
                return Ok(result);
            }
        }
        
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
}