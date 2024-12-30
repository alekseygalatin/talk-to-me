using System.Text.Json;
using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.Agents;
using TalkToMe.Core.Interfaces;
using TalkToMe.Models;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private SwedishConversationAgent _swedishConversationAgent;
    private SwedishTranslationAgent _swedishTranslationAgent;
    private SwedishStoryTailorAgent _swedishStoryTailorAgent;
    private SwedishRetailerAgent _swedishRetailerAgent;
    private SwedishConversationHelperAgent _swedishConversationHelperAgent;
    private SwedishWordTeacherAgent _swedishWordTeacherAgent;
    
    private EnglishConversationAgent _englishConversationAgent;
    private EnglishTranslationAgent _englishTranslationAgent;
    private EnglishStoryTailorAgent _englishStoryTailorAgent;
    private EnglishRetailerAgent _englishRetailerAgent;
    private EnglishConversationHelperAgent _englishConversationHelperAgent;
    private EnglishWordTeacherAgent _englishWordTeacherAgent;
        
    private readonly AmazonPollyClient _pollyClient;
    private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        
    public AgentsController(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager, IWordService wordService)
    {
        _swedishConversationAgent = new SwedishConversationAgent(aiProviderFactory, conversationManager);
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory);
        _swedishStoryTailorAgent = new SwedishStoryTailorAgent(aiProviderFactory);
        _swedishRetailerAgent = new SwedishRetailerAgent(aiProviderFactory);
        _swedishConversationHelperAgent = new SwedishConversationHelperAgent(aiProviderFactory, conversationManager);
        _swedishWordTeacherAgent = new SwedishWordTeacherAgent(aiProviderFactory, conversationManager, wordService);
            
        _englishConversationAgent = new EnglishConversationAgent(aiProviderFactory, conversationManager);
        _englishTranslationAgent = new EnglishTranslationAgent(aiProviderFactory);
        _englishStoryTailorAgent = new EnglishStoryTailorAgent(aiProviderFactory);
        _englishRetailerAgent = new EnglishRetailerAgent(aiProviderFactory);
        _englishConversationHelperAgent = new EnglishConversationHelperAgent(aiProviderFactory, conversationManager);
        _englishWordTeacherAgent = new EnglishWordTeacherAgent(aiProviderFactory, conversationManager, wordService);
        
        _pollyClient = new AmazonPollyClient(bucketRegion);
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
                
                var response = await _swedishConversationAgent.Invoke(text, sub);
                return this.CreateResponse(response.Response);
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                    text = "Hi";
                
                var response = await _englishConversationAgent.Invoke(text, sub);
                return this.CreateResponse(response.Response);
            }
        }
        else if (agent == "wordTeacherAgent")
        {
            if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(text))
                    text = "Hej";
                
                var response = await _swedishWordTeacherAgent.Invoke(text, sub);
                return this.CreateResponse(response.Response);
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                    text = "Hi";
                
                var response = await _englishWordTeacherAgent.Invoke(text, sub);
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
                var response = await _swedishConversationHelperAgent.Invoke(text, sub);
                return this.CreateResponse(response.Response);
            }
            else
            {
                var response = await _englishConversationHelperAgent.Invoke(text, sub);
                return this.CreateResponse(response.Response);
            }
        }
        
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
    
    [HttpPost("{locale}/{agent}/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string locale, [FromRoute] string agent)
    {
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