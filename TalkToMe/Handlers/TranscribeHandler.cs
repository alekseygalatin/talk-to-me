using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
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
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
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
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient; // Cognito client
    private readonly MemoryCache _cache;
    private static readonly AmazonBedrockAgentRuntimeClient _bedrockRuntime = new AmazonBedrockAgentRuntimeClient(RegionEndpoint.USEast1);

    public TranscribeHandler()
    {
        _s3Client = new AmazonS3Client(bucketRegion);
        _transcribeClient = new AmazonTranscribeServiceClient(bucketRegion);
        _pollyClient = new AmazonPollyClient(bucketRegion);
        _cognitoClient = new AmazonCognitoIdentityProviderClient(bucketRegion); // Initialize Cognito client
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> ProcessText(APIGatewayHttpApiV2ProxyRequest request)
    {
        if (request.RequestContext.Http.Method == "OPTIONS")
        {
            return CreateCorsResponse();
        }
        
        string token = request.Headers.TryGetValue("authorization", out var authHeader) ? authHeader : null;

        var tuple = await ValidateCognitoToken(token);
        if (!tuple.Item1)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 401,
                Body = "Unauthorized"
            };
        }
        
        var sub = tuple.Item2.Claims.First(x => x.Type.Equals("sub")).Value;

        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
        Console.WriteLine(dictionary?.GetValueOrDefault("text") ?? "");
        
        string bedrockResponse = await SendTextToBedrock(dictionary?.GetValueOrDefault("text") ?? "", sub);
        byte[] responseBytes = await ConvertTextToSpeechSwedish(bedrockResponse);

        var response = CreateResponse(responseBytes, bedrockResponse);
        return response;
    }
    
    private APIGatewayHttpApiV2ProxyResponse CreateCorsResponse()
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Content-Disposition", "attachment; filename=\"audio.wav\"" }, 
                { "Access-Control-Allow-Origin", "https://d3u8od6g4wwl6c.cloudfront.net" },
                // { "Access-Control-Allow-Origin", "http://localhost:5173" },
                { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
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

        string token = request.Headers.TryGetValue("Authorization", out var authHeader) ? authHeader : null;

        var tuple = await ValidateCognitoToken(token);
        if (!tuple.Item1)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 401,
                Body = "Unauthorized"
            };
        }

        var sub = tuple.Item2.Claims.First(x => x.Type.Equals("sub")).Value;

        Console.WriteLine(JsonSerializer.Serialize(request));
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
        string base64Audio = dictionary?.GetValueOrDefault("audio") ?? "";
    
        byte[] audioBytes = Convert.FromBase64String(base64Audio);
        using var audioStream = new MemoryStream(audioBytes);
        var mp3InputFileKey = Guid.NewGuid().ToString();
    
        await UploadFileToS3(audioStream, mp3InputFileKey);
        string transcriptText = await TranscribeMp3ToText(mp3InputFileKey);
        string bedrockResponse = await SendTextToBedrock(transcriptText, sub);
        byte[] responseBytes = await ConvertTextToSpeech(bedrockResponse);
    
        return CreateResponse(responseBytes, bedrockResponse);
    }
    
    public static APIGatewayHttpApiV2ProxyResponse CreateResponse(byte[] audioBytes, string text)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Content-Disposition", "attachment; filename=\"audio.wav\"" },
                { "Access-Control-Allow-Origin", "https://d3u8od6g4wwl6c.cloudfront.net" },
                // { "Access-Control-Allow-Origin", "http://localhost:5173" },
                { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
                { "Access-Control-Allow-Methods", "POST" }
            },
            Body = JsonSerializer.Serialize(new
            {
                Audio = Convert.ToBase64String(audioBytes),
                Text = text
            })
        };
    }

    private async Task UploadFileToS3(MemoryStream audioStream, string fileKey)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);
        audioStream.Position = 0; // Reset stream position before upload
        await fileTransferUtility.UploadAsync(audioStream, s3BucketName, fileKey);
    }

    private async Task<string> SendTextToBedrock(string chat, string sessionId)
    {
        var response = await _bedrockRuntime.InvokeAgentAsync(new InvokeAgentRequest
        {
            AgentAliasId = "Y16RG8SL7P",
            AgentId = "B1KZSIORTC",
            InputText = chat,
            SessionId = sessionId
        });

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            MemoryStream output = new MemoryStream();
            foreach (PayloadPart item in response.Completion)
            {
                item.Bytes.CopyTo(output);
            }

            var responseText = Encoding.UTF8.GetString(output.ToArray());
            Console.WriteLine("RES: " + responseText);
            // try
            // {
            //     var result = JsonConvert.DeserializeObject<AgenQuestionResponseModel>(responseText);
            //     responseText = result.Parameters.Topic ?? result.Parameters.Question;
            // }
            // catch
            // {
            // }
            
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
            OutputFormat = OutputFormat.Pcm,
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

    private async Task<(bool, JwtSecurityToken)> ValidateCognitoToken(string token)
    {
        // Replace with your User Pool ID, Region, and App Client ID
        string userPoolId = "us-east-1_walDCpNcK"; // Your User Pool ID
        string region = "us-east-1"; // Change to your region
        string clientId = "7o8tqlt2ucihqsbtthfopc9d4p"; // Your App Client ID without a secret

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Token is null or empty.");
            return (false, null); // Early return for invalid token input
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var keys = await GetCognitoPublicKeysAsync(userPoolId, region);
            
            // Specify token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys,
                ValidateIssuer = true,
                ValidIssuer = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}",
                ValidateAudience = true,
                ValidAudience = clientId,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Do not allow any clock skew
            };

            // Validate the token
            handler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            
            // If we reach this point, the token is valid
            return (true, validatedToken as JwtSecurityToken);
        }
        catch (SecurityTokenExpiredException)
        {
            Console.WriteLine("Token is expired.");
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            Console.WriteLine("Token signature is invalid.");
        }
        catch (SecurityTokenInvalidAudienceException)
        {
            Console.WriteLine("Token audience is invalid.");
        }
        catch (SecurityTokenInvalidIssuerException)
        {
            Console.WriteLine("Token issuer is invalid.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
        }

        return (false, null); // Token is invalid or other error
    }

    private async Task<IEnumerable<JsonWebKey>> GetCognitoPublicKeysAsync(string userPoolId, string region)
    {
        // Fetch the Cognito public keys from the AWS Cognito JWK endpoint
        var jwksUri = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}/.well-known/jwks.json";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(jwksUri);
        
        // Deserialize directly into the JsonWebKeySet
        var keys = JsonSerializer.Deserialize<JsonWebKeySet>(response);

        return keys.Keys;
    }
}
