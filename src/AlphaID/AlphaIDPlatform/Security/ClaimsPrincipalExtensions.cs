using IdentityModel;
using System.Security.Claims;
using System.Text;

namespace AlphaIDPlatform.Security;

/// <summary>
/// Extensions for ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// 获取用户的Profile URL.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? ProfileUrl(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(JwtClaimTypes.Profile);
    }

    /// <summary>
    /// 获取用户的Avatar Url.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? AvatarUrl(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(JwtClaimTypes.Picture);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? SubjectId(this ClaimsPrincipal principal)
    {
        var subjectIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        subjectIdValue ??= principal.FindFirstValue(JwtClaimTypes.Subject);
        return subjectIdValue;
    }

    public static string? DisplayRoles(this ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity claimsIdentity)
        {
            return null;
        }
        var roleClaims = principal.Claims.Where(c => c.Type == claimsIdentity.RoleClaimType).ToArray();
        var sb = new StringBuilder();
        foreach (var roleClaim in roleClaims)
        {
            sb.Append($", {roleClaim.Value}");
        }
        //roleClaims.Select(c => sb.Append($", {c.Value}"));
        return sb.ToString().Trim(',', ' ');
    }
}
