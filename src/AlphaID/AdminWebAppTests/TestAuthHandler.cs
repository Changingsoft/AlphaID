using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AdminWebAppTests;
internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (this.Request.Headers.Authorization.Contains("TestScheme"))
        {
            var claims = new List<Claim>()
            {
                new Claim("http://schemas.microsoft.com/claims/authnmethodsreferences", "pwd"),
                new Claim("sid", "9ABAFC649FB347031416CEC996E314F2"),
                new Claim(ClaimTypes.NameIdentifier, "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new Claim("auth_time", "1697701998"),
                new Claim("http://schemas.microsoft.com/identity/claims/identityprovider", "local"),
                new Claim("name", "刘备"),
                new Claim("given_name", "备"),
                new Claim("family_name", "刘"),
                new Claim("profile", "https://localhost:49726/Profile"),
                new Claim(ClaimTypes.Name, "刘备"),
                new Claim("picture", "https://localhost:49726/Profile/Avatar"),
                new Claim("locale", "zh-CN"),
                new Claim("zoneinfo", "Asia/Shanghai"),
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
