using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AlphaIdWebAPI.Tests;
internal class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.Authorization.Contains("Bearer"))
        {
            var claims = new List<Claim>()
            {
                new("iss", "https://issuer.example.com"),
                new("nbf", "1697709199"),
                new("iat", "1697709199"),
                new("exp", "1697712799"),
                new("aud", "https://example.com"),
                new("scope", "openid"),
                new("scope", "profile"),
                new("scope", "realname"),
                new("http://schemas.microsoft.com/claims/authnmethodsreferences", "pwd"),
                new("client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c"),
                new(ClaimTypes.NameIdentifier, "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new("auth_time", "1697709198"),
                new("http://schemas.microsoft.com/identity/claims/identityprovider", "local"),
                new("sid", "9ABAFC649FB347031416CEC996E314F2"),
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Bearer");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
