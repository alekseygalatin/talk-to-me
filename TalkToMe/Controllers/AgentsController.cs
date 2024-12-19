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

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private SwedishConversationAgent _swedishConversationAgent;
    private SwedishTranslationAgent _swedishTranslationAgent;
    private SwedishStoryTailorAgent _storyTailorAgent;
    private SwedishRetailerAgent _swedishRetailerAgent;
    private SwedishConversationHelperAgent _conversationHelperAgent;
    private SwedishWordTeacherAgent _swedishWordTeacherAgent;
        
    private readonly AmazonPollyClient _pollyClient;
    private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        
    public AgentsController(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager, IWordService wordService)
    {
        _swedishConversationAgent = new SwedishConversationAgent(aiProviderFactory, conversationManager);
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory);
        _storyTailorAgent = new SwedishStoryTailorAgent(aiProviderFactory);
        _swedishRetailerAgent = new SwedishRetailerAgent(aiProviderFactory);
        _conversationHelperAgent = new SwedishConversationHelperAgent(aiProviderFactory, conversationManager);
        _swedishWordTeacherAgent = new SwedishWordTeacherAgent(aiProviderFactory, conversationManager, wordService);
            
        _pollyClient = new AmazonPollyClient(bucketRegion);
    }
        
    [HttpPost("{agent}/text/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string agent, [FromBody] string text)
    {
        var sub = "123";//this.HttpContext.User.Claims.First(x => x.Type.Equals("sub")).Value;
        if (agent == "conversationAgent")
        {
            var response = await _swedishConversationAgent.Invoke(text, sub);
            var audio = await ConvertTextToSpeechSwedish(response.Response);
            return this.CreateResponse(audio, response.Response);
        }
        else if (agent == "wordTeacherAgent")
        {
            var response = await _swedishWordTeacherAgent.Invoke(text, sub);
            var audio = await ConvertTextToSpeechSwedish(response.Response);
            return this.CreateResponse(audio, response.Response);
        }
        else if (agent == "translationAgent")
        {
            var response = await _swedishTranslationAgent.Invoke(text);
            return this.CreateResponse(null, response.Response);
        }
        else if (agent == "conversationHelperAgent")
        {
            var response = await _conversationHelperAgent.Invoke(text, sub);
            return this.CreateResponse(null, response.Response);
        }
        
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
    
    [HttpPost("{agent}/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string agent)
    {
        if (agent == "storyTailorAgent")
        {
            var response = await _storyTailorAgent.Invoke();
            var audio = await ConvertTextToSpeechSwedish(response.Response);
            return this.CreateResponse(audio, response.Response);
        }

        throw new NotFoundException($"Agent: {agent} has not been found");
    }
    
    [HttpPost("{agent}/promt/text/invoke")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Invoke([FromRoute] string agent, [FromBody] WithPromtRequest data)
    {
        if (agent == "retailerAgent")
        {
            var response = await _swedishRetailerAgent.Invoke(data.Promt, data.Text);
            var audio = await ConvertTextToSpeechSwedish(response.Response);
            return this.CreateResponse(audio, response.Response);
        }
        
        throw new NotFoundException($"Agent: {agent} has not been found");
    }
        
    private async Task<byte[]> ConvertTextToSpeechSwedish(string text)
    {
        var synthesizeSpeechRequest = new SynthesizeSpeechRequest
        {
            OutputFormat = OutputFormat.Mp3,
            Text = text,
            VoiceId = "Elin", // Neural Swedish voice
            Engine = Engine.Neural // Use the neural engine
        };

        using var synthesizeSpeechResponse = await _pollyClient.SynthesizeSpeechAsync(synthesizeSpeechRequest);
        using var memoryStream = new MemoryStream();
        await synthesizeSpeechResponse.AudioStream.CopyToAsync(memoryStream);

        return memoryStream.ToArray(); // Return the audio byte array
    }
        
    private APIGatewayHttpApiV2ProxyResponse CreateResponse(byte[]? audioBytes, string text)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new
            {
                Audio = audioBytes != null ? Convert.ToBase64String(audioBytes): "",
                Text = text
            })
        };
    }
}