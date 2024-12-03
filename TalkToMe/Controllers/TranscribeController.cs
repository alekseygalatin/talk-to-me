using System.Text.Json;
using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkToMe.Core.Agents;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TranscribeController : ControllerBase
{
    private SwedishConversationAgent _swedishConversationAgent;
    private SwedishTranslationAgent _swedishTranslationAgent;
        
    private readonly AmazonPollyClient _pollyClient;
    private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        
    public TranscribeController(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager)
    {
        _swedishConversationAgent = new SwedishConversationAgent(aiProviderFactory, conversationManager);
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory);
            
        _pollyClient = new AmazonPollyClient(bucketRegion);
    }
        
    [HttpPost("process-text")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> ProcessText([FromBody] string text)
    {
        var sub = this.HttpContext.User.Claims.First(x => x.Type.Equals("aud")).Value;
        var response = await _swedishConversationAgent.Invoke(text, sub);
        var audio = await ConvertTextToSpeechSwedish(response.Response);
        return this.CreateResponse(audio, response.Response);
    }
        
    [HttpPost("translate-word")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> TranslateWord([FromBody] string text)
    {
        var response = await _swedishTranslationAgent.Invoke(text);
        return this.CreateResponse(null, response.Response);
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