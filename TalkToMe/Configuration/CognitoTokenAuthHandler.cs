using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TalkToMe.Configuration;

public class CognitoTokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public CognitoTokenAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public CognitoTokenAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string token = Request.Headers.TryGetValue("authorization", out var authHeader) ? authHeader.ToString() : null;

        if (string.IsNullOrEmpty(token))
        {
            return await Task.FromResult(AuthenticateResult.Fail("Token is empty!"));
        }
        
        var principal = await ValidateCognitoToken(token);

        var tiket = new AuthenticationTicket(principal, Scheme.Name);
        return await Task.FromResult(AuthenticateResult.Success(tiket));
    }
    
    private async Task<ClaimsPrincipal> ValidateCognitoToken(string token)
    {
        // Replace with your User Pool ID, Region, and App Client ID
        string userPoolId = "us-east-1_walDCpNcK"; // Your User Pool ID
        string region = "us-east-1"; // Change to your region
        string clientId = "7o8tqlt2ucihqsbtthfopc9d4p"; // Your App Client ID without a secret

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Token is null or empty.");
            return null; // Early return for invalid token input
        }

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
        return handler.ValidateToken(token, tokenValidationParameters, out _);
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