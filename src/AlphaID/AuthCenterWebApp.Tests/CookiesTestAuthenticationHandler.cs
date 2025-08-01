using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AuthCenterWebApp.Tests;

internal class CookiesTestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.Authorization.Contains("Cookies"))
        {
            var claims = new List<Claim>
            {
                new("sub", "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new("email", "liubei@sanguo.net"),
                new("AspNet.Identity.SecurityStamp", "JLLTBHU57I6HIHYOIMQBBDO4IOEZURXG"),
                new("name", "刘备"),
                new("profile", "https://localhost:49726/Profile"),
                new("picture", "https://localhost:49726/Profile/Avatar"),
                new("updated_at", "1611100800"),
                new("locale", "zh-CN"),
                new("zoneinfo", "Asia/Shanghai"),
                new("given_name", "备"),
                new("family_name", "刘"),
                new("nickname", "备备"),
                new("address", "中国,河北省,涿州,,北京路,,,,刘备,"),
                new("search_hint", "LIUBEI"),
                new("birthdate", "0161-07-16"),
                new("phone_number", "+8613812340001"),
                new("phone_number_verified", "True"),
                new("gender", "male"),
                new("email", "liubei@sanguo.net"),
                new("email_verified", "True"),
                new("preferred_username", "liubei@sanguo.net"),
                new("preferred_username", "liubei@sanguo.net"),
                new("email_verified", "true"),
                new("phone_number", "+8613812340001"),
                new("phone_number_verified", "true"),
                new("amr", "pwd"),
                new("idp", "local"),
                new("auth_time", "1697725253")
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Cookies");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}