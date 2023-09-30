using IdentityModel;
using System.Security.Claims;

namespace IDSubjects;

/// <summary>
/// Extensions for <see cref="PersonInfo"/>
/// </summary>
public static class PersonInfoExtensions
{
    /// <summary>
    /// Get <see cref="PersonInfo"/> from <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static PersonInfo GetPersonInfo(this ClaimsPrincipal principal)
    {
        var id = (principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue(JwtClaimTypes.Subject)) ?? throw new InvalidOperationException("无法从用户声明中找到Id");
        var name = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue(JwtClaimTypes.Name) ?? throw new InvalidOperationException("无法从用户声明中找到名称");
        var hint = principal.FindFirstValue("phonetic_search_hint");
        return new PersonInfo(id, name, hint);
    }
}
