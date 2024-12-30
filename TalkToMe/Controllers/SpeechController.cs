using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TalkToMe.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class SpeechController : ControllerBase
{
    private readonly AmazonPollyClient _pollyClient;
    private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        
    public SpeechController()
    {
        _pollyClient = new AmazonPollyClient(bucketRegion);
    }
    
    [HttpPost("{locale}/speech")]
    public async Task<ActionResult> Invoke([FromRoute] string locale, [FromBody] string text)
    {
        if (locale.Equals("sv-se", StringComparison.OrdinalIgnoreCase))
        {
            var audio = await ConvertTextToSpeechSwedish(text);
            return Ok(Convert.ToBase64String(audio));
        }
        else
        {
            var audio = await ConvertTextToSpeechEnglish(text);
            return Ok(Convert.ToBase64String(audio));
        }
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
    
    private async Task<byte[]> ConvertTextToSpeechEnglish(string text)
    {
        var synthesizeSpeechRequest = new SynthesizeSpeechRequest
        {
            OutputFormat = OutputFormat.Mp3,
            Text = text,
            VoiceId = "Ruth", // Neural English voice
            Engine = Engine.Neural // Use the neural engine
        };

        using var synthesizeSpeechResponse = await _pollyClient.SynthesizeSpeechAsync(synthesizeSpeechRequest);
        using var memoryStream = new MemoryStream();
        await synthesizeSpeechResponse.AudioStream.CopyToAsync(memoryStream);

        return memoryStream.ToArray(); // Return the audio byte array
    }
}