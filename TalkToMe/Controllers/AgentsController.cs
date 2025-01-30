using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.Agents;
using TalkToMe.Core.Agents.Aws;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Models;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private SwedishTranslationAgent _swedishTranslationAgent;
    private SwedishStoryTailorAgent _swedishStoryTailorAgent;
    private SwedishRetailerAgent _swedishRetailerAgent;
    private SwedishConversationHelperAgent _swedishConversationHelperAgent;
    private SwedishAlexAgent _swedishAlexAgent;
    private SwedishEmmaAgent _swedishEmmaAgent;
    
    private EnglishTranslationAgent _englishTranslationAgent;
    private EnglishStoryTailorAgent _englishStoryTailorAgent;
    private EnglishRetailerAgent _englishRetailerAgent;
    private EnglishConversationHelperAgent _englishConversationHelperAgent;
    private EnglishAlexAgent _englishAlexAgent;
    private EnglishEmmaAgent _englishEmmaAgent;
        
    public AgentsController(IAIProviderFactory aiProviderFactory, IHistoryService historyService, IWordService wordService, IBedrockAgentService bedrockAgentService)
    {
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory);
        _swedishStoryTailorAgent = new SwedishStoryTailorAgent(aiProviderFactory);
        _swedishRetailerAgent = new SwedishRetailerAgent(aiProviderFactory);
        _swedishConversationHelperAgent = new SwedishConversationHelperAgent(aiProviderFactory);
        _swedishAlexAgent = new SwedishAlexAgent(bedrockAgentService, historyService);
        _swedishEmmaAgent = new SwedishEmmaAgent(bedrockAgentService, wordService, historyService);
        
        _englishTranslationAgent = new EnglishTranslationAgent(aiProviderFactory);
        _englishStoryTailorAgent = new EnglishStoryTailorAgent(aiProviderFactory);
        _englishRetailerAgent = new EnglishRetailerAgent(aiProviderFactory);
        _englishConversationHelperAgent = new EnglishConversationHelperAgent(aiProviderFactory);
        _englishAlexAgent = new EnglishAlexAgent(bedrockAgentService, historyService);
        _englishEmmaAgent = new EnglishEmmaAgent(bedrockAgentService, wordService, historyService);
    }
        
    [HttpPost("{locale}/{agent}/text/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string locale, [FromRoute] string agent, [FromBody] string text)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        if (agent == "conversationAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(text))
                    text = "Hej";
                
                var response = await _swedishAlexAgent.Invoke(text, sub);
                return this.CreateResponse(response.Response);
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                    text = "Hi";
                
                var response = await _englishAlexAgent.Invoke(text, sub);
                return this.CreateResponse(response.Response);
            }
        }
        else if (agent == "wordTeacherAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                CoreResponse response;
                if (string.IsNullOrWhiteSpace(text))
                {
                    response = await _swedishEmmaAgent.InitialInvoke(sub);
                }
                else
                {
                    response = await _swedishEmmaAgent.Invoke(text, sub);
                }
                
                return this.CreateResponse(response.Response);
            }
            else
            {
                CoreResponse response;
                if (string.IsNullOrWhiteSpace(text))
                {
                    response = await _englishEmmaAgent.InitialInvoke(sub);
                }
                else
                {
                    response = await _englishEmmaAgent.Invoke(text, sub);
                }
                
                return this.CreateResponse(response.Response);
            }
        }
        else if (agent == "translationAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _swedishTranslationAgent.Invoke(text);
                return this.CreateResponse(response.Response);
            }
            else
            {
                var response = await _englishTranslationAgent.Invoke(text);
                return this.CreateResponse(response.Response);
            }
        }
        else if (agent == "conversationHelperAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _swedishConversationHelperAgent.Invoke(text);
                return this.CreateResponse(response.Response);
            }
            else
            {
                var response = await _englishConversationHelperAgent.Invoke(text);
                return this.CreateResponse(response.Response);
            }
        }
        
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
    
    [HttpPost("{locale}/{agent}/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string locale, [FromRoute] string agent)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        if (agent == "storyTailorAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _swedishStoryTailorAgent.Invoke();
                return this.CreateResponse(response.Response);
            }
            else
            {
                var response = await _englishStoryTailorAgent.Invoke();
                return this.CreateResponse(response.Response);
            }
        }

        throw new NotFoundException($"Agent: {agent} has not been found");
    }
    
    [HttpPost("{locale}/{agent}/promt/text/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string locale, [FromRoute] string agent, [FromBody] WithPromtRequest data)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        if (agent == "retailerAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _swedishRetailerAgent.Invoke(data.Promt, data.Text);
                return this.CreateResponse(response.Response);
            }
            else
            {
                var response = await _englishRetailerAgent.Invoke(data.Promt, data.Text);
                return this.CreateResponse(response.Response);
            }
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