using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Amazon;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.IdentityModel.Tokens;
using JsonSerializer = System.Text.Json.JsonSerializer;
using PayloadPart = Amazon.BedrockAgentRuntime.Model.PayloadPart;

namespace TalkToMe;

public class TranslationHandler
{
    private static readonly AmazonBedrockAgentRuntimeClient _bedrockRuntime = new AmazonBedrockAgentRuntimeClient(RegionEndpoint.USEast1);

    public TranslationHandler()
    {
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
        var response = await _bedrockRuntime.InvokeAgentAsync(new InvokeAgentRequest
        {
            AgentAliasId = "T6D1PSJSZR",
            AgentId = "VPE9UHNC8T",
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
