using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AlphaIDWebAPITests;
internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if(this.Request.Headers.Authorization.Contains("TestBearer"))
        {
            var claims = new List<Claim>()
            {
                new Claim("iss", "https://issuer.example.com"),
                new Claim("nbf", "1697709199"),
                new Claim("iat", "1697709199"),
                new Claim("exp", "1697712799"),
                new Claim("aud", "https://example.com"),
                new Claim("scope", "openid"),
                new Claim("scope", "profile"),
                new Claim("http://schemas.microsoft.com/claims/authnmethodsreferences", "pwd"),
                new Claim("client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c"),
                new Claim(ClaimTypes.NameIdentifier, "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new Claim("auth_time", "1697709198"),
                new Claim("http://schemas.microsoft.com/identity/claims/identityprovider", "local"),
                new Claim("sid", "9ABAFC649FB347031416CEC996E314F2"),
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
