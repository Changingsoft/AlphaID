using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AuthCenterWebApp.Tests;
internal class BearerTestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{


    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            var claims = new List<Claim>
            {
                new("sub", "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new(ClaimTypes.NameIdentifier, "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new("updated_at", "1611100800"),
                new("locale", "zh-CN"),
                new("zoneinfo", "Asia/Shanghai"),
                new("amr", "pwd"),
                new("idp", "local"),
                new("auth_time", "1697725253"),
                new("scope", "membership")
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, JwtBearerDefaults.AuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
