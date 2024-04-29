using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdminWebApp.Tests;

internal class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.Authorization.Contains("TestScheme"))
        {
            var claims = new List<Claim>
            {
                new("http://schemas.microsoft.com/claims/authnmethodsreferences", "pwd"),
                new("sid", "9ABAFC649FB347031416CEC996E314F2"),
                new(ClaimTypes.NameIdentifier, "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new("auth_time", "1697701998"),
                new("http://schemas.microsoft.com/identity/claims/identityprovider", "local"),
                new("name", "刘备"),
                new("given_name", "备"),
                new("family_name", "刘"),
                new("profile", "https://localhost:49726/Profile"),
                new(ClaimTypes.Name, "刘备"),
                new("picture", "https://localhost:49726/Profile/Avatar"),
                new("locale", "zh-CN"),
                new("zoneinfo", "Asia/Shanghai")
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}