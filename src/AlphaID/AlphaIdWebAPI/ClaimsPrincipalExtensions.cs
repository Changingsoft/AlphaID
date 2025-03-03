using System.Security.Claims;
using IdentityModel;

namespace AlphaIdWebAPI;

/// <summary>
/// Extensions for ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// 获取ClientId声明的值。
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? ClientId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(JwtClaimTypes.ClientId);
    }
}