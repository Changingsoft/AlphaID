using AlphaIdPlatform.Admin;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace AdminWebApp.Services;

public class OidcEvents
{
    public static Task IssueRoleClaims(TokenValidatedContext context)
    {
        var user = context.Principal ?? throw new InvalidOperationException("No principal.");
        if (user.Identity is { IsAuthenticated: true })
        {
            ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
            var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var userInRoleManager = context.HttpContext.RequestServices.GetRequiredService<UserInRoleManager>();
                var roles = userInRoleManager.GetRoles(userId);
                identity.AddClaims(from role in roles
                                   select new Claim(identity.RoleClaimType, role));
            }
        }
        return Task.CompletedTask;
    }
}
