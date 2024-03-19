using IdentityModel;
using System.Security.Claims;

namespace IdSubjects;
internal class DefaultSubjectGenerator : ISubjectGenerator
{
    public string Generate(ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? principal.FindFirstValue(JwtClaimTypes.Subject)
               ?? throw new InvalidOperationException("找不到有效的 subject 声明。");
    }
}
