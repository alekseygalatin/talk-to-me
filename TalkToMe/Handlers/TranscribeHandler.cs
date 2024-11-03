using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using TalkToMe.Handlers;
using JsonSerializer = System.Text.Json.JsonSerializer;
using PayloadPart = Amazon.BedrockAgentRuntime.Model.PayloadPart;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace TalkToMe;

public class TranscribeHandler
{
    private readonly string s3BucketName = "talktomebucket";
    private readonly string transcribeJobName = "transcription-job";
    private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
    private readonly AmazonS3Client _s3Client;
    private readonly AmazonTranscribeServiceClient _transcribeClient;
    private readonly AmazonPollyClient _pollyClient;
    private readonly MemoryCache _cache;
    private static readonly AmazonBedrockAgentRuntimeClient _bedrockRuntime = new AmazonBedrockAgentRuntimeClient(RegionEndpoint.USEast1);

    public TranscribeHandler()
    {
        _s3Client = new AmazonS3Client(bucketRegion);
        _transcribeClient = new AmazonTranscribeServiceClient(bucketRegion);
        _pollyClient = new AmazonPollyClient(bucketRegion);
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> ProcessText(APIGatewayHttpApiV2ProxyRequest request)
    {
        if (request.RequestContext.Http.Method == "OPTIONS")
        {
            return CreateCorsResponse();
        }

        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);

        Console.WriteLine(dictionary?.GetValueOrDefault("text") ?? "");
        
        string bedrockResponse = await SendTextToBedrock(dictionary?.GetValueOrDefault("text") ?? "");
        Console.WriteLine("RESPONSE: " + bedrockResponse);
        byte[] responseBytes = await ConvertTextToSpeechSwedish(bedrockResponse);

        var response = CreateResponse(responseBytes);
        return response;
    }
    
    private APIGatewayHttpApiV2ProxyResponse CreateCorsResponse()
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "audio/wav" },
                { "Content-Disposition", "attachment; filename=\"audio.wav\"" },
                { "Access-Control-Allow-Origin", "https://d3u8od6g4wwl6c.cloudfront.net" },  // Allow any origin
                { "Access-Control-Allow-Headers", "Content-Type" },  // Allow Content-Type header
                { "Access-Control-Allow-Methods", "POST" }
            }
        };
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> ProcessAudio(APIGatewayHttpApiV2ProxyRequest request)
    {
        if (request.RequestContext.Http.Method == "OPTIONS")
        {
            return CreateCorsResponse();
        }
        
        Console.WriteLine(JsonSerializer.Serialize(request));
        Console.WriteLine(request.Body);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
        // using var inputStream = new MemoryStream(Convert.FromBase64String(dictionary?.GetValueOrDefault("audio") ?? ""));
        // using var reader = new StreamReader(inputStream);
        string base64Audio = dictionary?.GetValueOrDefault("audio") ?? "";
    
        byte[] audioBytes = Convert.FromBase64String(base64Audio);
        using var audioStream = new MemoryStream(audioBytes);
        var mp3InputFileKey = Guid.NewGuid().ToString();
    
        await UploadFileToS3(audioStream, mp3InputFileKey);
        string transcriptText = await TranscribeMp3ToText(mp3InputFileKey);
        string bedrockResponse = await SendTextToBedrock(transcriptText);
        byte[] responseBytes = await ConvertTextToSpeech(bedrockResponse);
    
        return CreateResponse(responseBytes);
    }
    
    public static APIGatewayHttpApiV2ProxyResponse CreateResponse(byte[] audioBytes)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "audio/wav" },
                { "Content-Disposition", "attachment; filename=\"audio.wav\"" },
                { "Access-Control-Allow-Origin", "https://d3u8od6g4wwl6c.cloudfront.net" },  // Allow any origin
                { "Access-Control-Allow-Headers", "Content-Type" },  // Allow Content-Type header
                { "Access-Control-Allow-Methods", "POST" }
            },
            Body = Convert.ToBase64String(audioBytes),
            IsBase64Encoded = true
        };
    }


    private async Task UploadFileToS3(MemoryStream audioStream, string fileKey)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);
        audioStream.Position = 0; // Reset stream position before upload
        await fileTransferUtility.UploadAsync(audioStream, s3BucketName, fileKey);
    }

    private async Task<string> SendTextToBedrock(string chat)
    {
        var response = await _bedrockRuntime.InvokeAgentAsync(new InvokeAgentRequest
        {
            AgentAliasId = "OA0NAMBO9F",
            AgentId = "WNWLTJJLDA",
            InputText = chat,
            SessionId = "123456789"
        });

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            MemoryStream output = new MemoryStream();
            foreach (PayloadPart item in response.Completion)
            {
                item.Bytes.CopyTo(output);
            }

            var responseText = Encoding.UTF8.GetString(output.ToArray());
            try
            {
                var result = JsonConvert.DeserializeObject<AgenQuestionResponseModel>(responseText);
                responseText = result.Parameters.Question;
            }
            catch
            {
            }
            
            return responseText;
        }

        throw new Exception("Failed to get response from Bedrock.");
    }

    private async Task<string> TranscribeMp3ToText(string mp3InputFileKey)
    {
        var startTranscriptionJobRequest = new StartTranscriptionJobRequest
        {
            TranscriptionJobName = transcribeJobName + Guid.NewGuid(),
            LanguageCode = "en-US",
            MediaFormat = "wav",
            Media = new Media
            {
                MediaFileUri = $"s3://{s3BucketName}/{mp3InputFileKey}"
            },
            OutputBucketName = s3BucketName
        };

        var transcriptionJobResponse = await _transcribeClient.StartTranscriptionJobAsync(startTranscriptionJobRequest);
        Console.WriteLine("Transcription job started...");

        TranscriptionJob transcriptionJob;
        do
        {
            await Task.Delay(2000); // Wait for job completion
            transcriptionJob = (await _transcribeClient.GetTranscriptionJobAsync(new GetTranscriptionJobRequest
            {
                TranscriptionJobName = startTranscriptionJobRequest.TranscriptionJobName
            })).TranscriptionJob;
        } while (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.IN_PROGRESS);

        if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
        {
            string transcriptFileKey = $"{startTranscriptionJobRequest.TranscriptionJobName}.json";
            string json = await GetTranscriptionTextFromS3(transcriptFileKey);
            dynamic transcriptionResult = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return transcriptionResult.results.transcripts[0].transcript;
        }

        throw new Exception("Transcription job failed.");
    }

    private async Task<string> GetTranscriptionTextFromS3(string transcriptFileKey)
    {
        var response = await _s3Client.GetObjectAsync(s3BucketName, transcriptFileKey);
        using (StreamReader reader = new StreamReader(response.ResponseStream))
        {
            return await reader.ReadToEndAsync();
        }
    }

    private async Task<byte[]> ConvertTextToSpeech(string text)
    {
        var synthesizeSpeechRequest = new SynthesizeSpeechRequest
        {
            OutputFormat = OutputFormat.Mp3,
            Text = text,
            VoiceId = "Joanna"
        };

        using var synthesizeSpeechResponse = await _pollyClient.SynthesizeSpeechAsync(synthesizeSpeechRequest);
        using var memoryStream = new MemoryStream();
        await synthesizeSpeechResponse.AudioStream.CopyToAsync(memoryStream);

        return memoryStream.ToArray(); // Return the audio byte array
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
}