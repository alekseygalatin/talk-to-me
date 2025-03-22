using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.Factories;
using TalkToMe.Helpers;
using TalkToMe.Models;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private AwsAgentFactory _agentFactory;
    public AgentsController(AwsAgentFactory agentFactory)
    {
        _agentFactory = agentFactory;
    }

    private string SessionId => UserHelper.GetUserId(User);
        
    [HttpPost("{locale}/{agent}/text/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string locale, [FromRoute] string agent, [FromBody] string text)
    {
        if (agent == "conversationAgent")
        {
            var instance = _agentFactory.GetAgent("alex", locale).WithMessage(text).WithSession(SessionId);
            var response = await instance.Invoke();
            return this.CreateResponse(response.Response);
        }
        else if (agent == "wordTeacherAgent")
        {
            var instance = _agentFactory.GetAgent("emma", locale);
            var response = await instance.WithMessage(text).WithSession(SessionId).Invoke();
            return this.CreateResponse(response.Response);
        }
        else if (agent == "translationAgent")
        {
            var instance = _agentFactory.GetAgent("translation", locale);
            var response = await instance.WithMessage(text).WithSession(SessionId).Invoke();
            return this.CreateResponse(response.Response);
        }
        else if (agent == "conversationHelperAgent")
        {
            var instance = _agentFactory.GetAgent("helper", locale);
            var response = await instance.WithMessage(text).WithSession(SessionId).Invoke();
            return this.CreateResponse(response.Response);
        }
        
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
    
    [HttpPost("{locale}/{agent}/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string locale, [FromRoute] string agent)
    {
        if (agent == "storyTailorAgent")
        {
            var instance = _agentFactory.GetAgent("maria", locale).WithSession(SessionId);
            var response = await instance.Invoke();
            
            var mariaChat = _agentFactory.GetAgent("maria-chat", locale);
            await mariaChat.WithMessage($"Här är originaltexten vi ska arbeta med: {response.Response}.").WithSession(SessionId).Invoke();
            
            return this.CreateResponse(response.Response);
        }

        throw new NotFoundException($"Agent: {agent} has not been found");
    }
    
    [HttpPost("{locale}/{agent}/promt/text/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string locale, [FromRoute] string agent, [FromBody] WithPromtRequest data)
    {
        if (agent == "retailerAgent")
        {
            var instance = _agentFactory.GetAgent("maria-chat", locale);
            var response = await instance.WithMessage($"Här är min återberättelse: {data.Text}.").WithSession(SessionId).Invoke();
                
            return this.CreateResponse(response.Response);
        }
        
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
        
    private APIGatewayHttpApiV2ProxyResponse CreateResponse(string text)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new
            {
                Text = text
            })
        };
    }
}