using IdentityModel;
using System.Globalization;
using System.Security.Claims;

namespace AlphaIdPlatform.Security;

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
        var url = principal.FindFirstValue(JwtClaimTypes.Picture);
        return url;
    }

    /// <summary>
    /// 确定声明中是否包括User profile picture.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static bool HasProfilePicture(this ClaimsPrincipal principal)
    {
        return principal.HasClaim(p => p.Type == JwtClaimTypes.Picture);
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

    /// <summary>
    /// 将用户的角色显示为逗号分隔的字符串
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? DisplayRoles(this ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity claimsIdentity)
        {
            return null;
        }
        var roleClaims = principal.Claims.Where(c => c.Type == claimsIdentity.RoleClaimType).ToArray();
        if (roleClaims.Length == 0)
            return string.Empty;
        return roleClaims.Select(p => p.Value).Aggregate((x, y) => $"{x},{y}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string DisplayName(this ClaimsPrincipal principal)
    {
        var surname = principal.FindFirstValue(JwtClaimTypes.FamilyName);
        var givenName = principal.FindFirstValue(JwtClaimTypes.GivenName);

        CultureInfo culture = CultureInfo.CurrentCulture;
        return culture.TwoLetterISOLanguageName switch
        {
            "zh" => $"{surname}{givenName}",
            "en" => $"{givenName} {surname}",
            _ => $"{givenName} {surname}"
        };
    }
}
