using System.Globalization;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Factories;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Enums;
using TalkToMe.Helpers;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    private IHistoryService _historyService;
    private AwsAgentFactory _agentFactory;
    
    public HistoryController(IHistoryService historyService, AwsAgentFactory agentFactory)
    {
        _historyService = historyService;
        _agentFactory = agentFactory;
    }
    
    private string SessionId => UserHelper.GetUserId(User);
        
    [HttpGet("{locale}/{agent}")]
    public async Task<ActionResult> Invoke([FromRoute] string locale, [FromRoute] string agent)
    {
        if (agent == "conversationAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var history = await _historyService.GetHistory(SessionId + "6a47a518-cc4b-4b40-b7a5-94113d8c1b16");
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
                var history = await _historyService.GetHistory(SessionId + "7b542e6e-7e2a-424f-8e76-0fec63cb2568");
                var result = history.Select(x => new HistoryMessageDto
                {
                    Message = x.Message,
                    IsUser = x.Role == ChatRole.User,
                    DateTime = DateTimeOffset.FromUnixTimeMilliseconds(x.Timestamp).UtcDateTime.ToString(CultureInfo.InvariantCulture)
                });
                return Ok(result);
            }
        } 
        else if (agent == "retailerAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var history = await _historyService.GetHistory(SessionId + "6e83c910-d4c2-491f-94ce-cd8f2c468c43");
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
                var history = await _historyService.GetHistory(SessionId + "a7568025-1326-493b-9c2d-070255881e08");
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

    [HttpDelete("{locale}/{agent}")]
    public async Task<ActionResult> CleanAgentMemory([FromRoute] string locale, [FromRoute] string agent)
    {
        if (agent == "conversationAgent")
        {
            var instance = _agentFactory
                .GetAgent("alex", locale)
                .WithSession(SessionId);
            
            await instance.CleanMemory();
        }
        else if (agent == "retailerAgent")
        {
            var instance = _agentFactory
                .GetAgent("maria-chat", locale)
                .WithSession(SessionId);

            await instance.CleanMemory();
        }

        return NoContent();
    }
}