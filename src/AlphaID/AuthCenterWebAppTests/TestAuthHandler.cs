using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AuthCenterWebAppTests;

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
                new Claim("sub", "d2480421-8a15-4292-8e8f-06985a1f645b"),
                new Claim("email", "liubei@sanguo.net"),
                new Claim("AspNet.Identity.SecurityStamp", "JLLTBHU57I6HIHYOIMQBBDO4IOEZURXG"),
                new Claim("name", "刘备"),
                new Claim("profile", "https://localhost:49726/Profile"),
                new Claim("picture", "https://localhost:49726/Profile/Avatar"),
                new Claim("updated_at", "1611100800"),
                new Claim("locale", "zh-CN"),
                new Claim("zoneinfo", "Asia/Shanghai"),
                new Claim("given_name", "备"),
                new Claim("family_name", "刘"),
                new Claim("nickname", "备备"),
                new Claim("address", "中国,河北省,涿州,,北京路,,,,刘备,"),
                new Claim("phonetic_search_hint", "LIUBEI"),
                new Claim("birthdate", "0161-07-16"),
                new Claim("phone_number", "+8613812340001"),
                new Claim("phone_number_verified", "True"),
                new Claim("gender", "male"),
                new Claim("email", "liubei@sanguo.net"),
                new Claim("email_verified", "True"),
                new Claim("preferred_username", "liubei@sanguo.net"),
                new Claim("preferred_username", "liubei@sanguo.net"),
                new Claim("email_verified", "true"),
                new Claim("phone_number", "+8613812340001"),
                new Claim("phone_number_verified", "true"),
                new Claim("amr", "pwd"),
                new Claim("idp", "local"),
                new Claim("auth_time", "1697725253"),
            };
            var identity = new ClaimsIdentity(claims, "AuthenticationTypes.Federation");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}