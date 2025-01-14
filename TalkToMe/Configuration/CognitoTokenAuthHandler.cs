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

    private readonly IMemoryCache _memoryCache;
    private readonly IHttpClientFactory _httpClientFactory;

    [Obsolete]
    public CognitoTokenAuthHandler(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
    }

    public CognitoTokenAuthHandler(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
        {
            return await Task.FromResult(AuthenticateResult.Fail("Token is empty!"));
        }
        
        var principal = await ValidateCognitoToken(authHeader!);
        if (principal is null)
        {
            return await Task.FromResult(AuthenticateResult.Fail("Invalid token validation parameters."));
        }

        var tiket = new AuthenticationTicket(principal, Scheme.Name);
        return await Task.FromResult(AuthenticateResult.Success(tiket));
    }
    
    private async Task<ClaimsPrincipal?> ValidateCognitoToken(string token)
    {
        // Replace with your User Pool ID, Region, and App Client ID
        const string userPoolId = "us-east-1_walDCpNcK"; // Your User Pool ID
        const string region = "us-east-1"; // Change to your region
        const string clientId = "7o8tqlt2ucihqsbtthfopc9d4p"; // Your App Client ID without a secret

        var tokenValidationParameters = await GetTokenValidationParameters(userPoolId, region, clientId);
        if (tokenValidationParameters is null)
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();

        return handler.ValidateToken(token, tokenValidationParameters, out _);
    }

    private async Task<TokenValidationParameters?> GetTokenValidationParameters(string userPoolId, string region, string clientId)
    {
        var cacheKey = (userPoolId, region, clientId);
        var tokenValidationParameters = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = RefreshCognitoPublicInterval;

            // Fetch the Cognito public keys from the AWS Cognito JWK endpoint
            var jwksUri = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}/.well-known/jwks.json";

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(jwksUri);
            var keySet = JsonSerializer.Deserialize<JsonWebKeySet>(response);

            if (keySet is null)
            {
                return null;
            }

            var keys = keySet.Keys;
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

            return tokenValidationParameters;
        });

        return tokenValidationParameters;
    }
}