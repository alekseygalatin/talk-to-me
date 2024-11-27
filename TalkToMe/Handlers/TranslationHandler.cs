using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Amazon;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.IdentityModel.Tokens;
using TalkToMe.Core.Builders;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Factories;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;
using PayloadPart = Amazon.BedrockAgentRuntime.Model.PayloadPart;

namespace TalkToMe;

public class TranslationHandler
{
    private static readonly AmazonBedrockAgentRuntimeClient _bedrockRuntime = new AmazonBedrockAgentRuntimeClient(RegionEndpoint.USEast1);

    private static IAiModelService _aiModelService;
    public TranslationHandler()
    {
        var settings = new BedrockSettings
        {
            Region = "us-east-1",
            DefaultModelId = "us.meta.llama3-1-8b-instruct-v1:0"
        };

        _aiModelService = new LamaAiModelService(new BedrockClientFactory(settings), settings, new ConversationManager(),
            BedrockAIModelNames.Lama3_1_8b_v1);
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
        var translateRequest = $"{dictionary?.GetValueOrDefault("text")}";
        string bedrockResponse = await SendTextToBedrock(translateRequest, sub);
        //byte[] responseBytes = await ConvertTextToSpeechSwedish(bedrockResponse);

        var response = CreateResponse(bedrockResponse);
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
                { "Access-Control-Allow-Origin", "https://talknlearn.com" },
                // { "Access-Control-Allow-Origin", "http://localhost:5173" },
                { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
                { "Access-Control-Allow-Methods", "POST" }
            }
        };
    }
    
    public static APIGatewayHttpApiV2ProxyResponse CreateResponse(string text)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "https://talknlearn.com" },
                // { "Access-Control-Allow-Origin", "http://localhost:5173" },
                { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
                { "Access-Control-Allow-Methods", "POST" }
            },
            Body = JsonSerializer.Serialize(new
            {
                //Audio = Convert.ToBase64String(audioBytes),
                Json = text
            })
        };
    }

    private async Task<string> SendTextToBedrock(string chat, string sessionId)
    {
        var request = new CoreRequestBuilder()
            .WithModel("us.meta.llama3-1-8b-instruct-v1:0")
            .WithSystemInstruction("You are a Swedish-to-English language translation agent. When given a Swedish word or phrase.\nProvide an accurate English translation, a brief example sentence showing natural usage in Swedish, and any relevant notes on nuances like article usage in Swedish.\n\nYour respons always must contain only JSON in the format: {\"translation\":  \"english translation\",  \"example_usage\": \"example swedish sentence\",  \"translation_notes\": \"notes on use\"}")
            .WithPrompt(chat)
            .Build();

        return (await _aiModelService.SendMessageAsync(request)).Response;
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
