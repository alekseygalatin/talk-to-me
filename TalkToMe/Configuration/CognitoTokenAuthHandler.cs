using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TalkToMe.Configuration;

public class CognitoTokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private static readonly TimeSpan RefreshCognitoPublicInterval = TimeSpan.FromMinutes(5);
    private static readonly Lazy<bool> IsDevelopment = new(() =>
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return !string.IsNullOrEmpty(environment) && environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
    });

    private readonly IMemoryCache _memoryCache;
    private readonly IHttpClientFactory _httpClientFactory;

    [Obsolete]
    public CognitoTokenAuthHandler(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
    }

    public CognitoTokenAuthHandler(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Headers.TryGetValue("authorization", out var authHeader) ? authHeader.ToString() : null;

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
        const string userPoolId = "us-east-1_walDCpNcK"; // Your User Pool ID
        const string region = "us-east-1"; // Change to your region
        const string clientId = "7o8tqlt2ucihqsbtthfopc9d4p"; // Your App Client ID without a secret

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Token is null or empty.");
            return null; // Early return for invalid token input
        }

        if (IsDevelopment.Value)
        {
            token = token.Replace("Bearer", "").Trim();
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
            ValidateAudience = false,
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

        var keys = await _memoryCache.GetOrCreateAsync(jwksUri, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = RefreshCognitoPublicInterval;

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(jwksUri);
            var keySet = JsonSerializer.Deserialize<JsonWebKeySet>(response);
            
            return keySet.Keys;
        });

        return keys;
    }
}