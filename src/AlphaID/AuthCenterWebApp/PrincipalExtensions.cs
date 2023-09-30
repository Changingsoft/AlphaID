using IdentityModel;
using System.Security.Claims;

namespace AuthCenterWebApp;

public static class PrincipalExtensions
{
    public static string? SubjectId(this ClaimsPrincipal principal)
    {
        var subjectIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        subjectIdValue ??= principal.FindFirstValue(JwtClaimTypes.Subject);
        return subjectIdValue;
    }
}
