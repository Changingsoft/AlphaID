using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace AlphaIDPlatform.Security;

/// <summary>
/// Claim transformation for role-based claims.
/// </summary>
public class ClaimTransformation : IClaimsTransformation
{
    private readonly IHostEnvironment environment;
    private readonly UserInRoleManager manager;

    /// <summary>
    /// Initialize.
    /// </summary>
    /// <param name="environment"></param>
    /// <param name="manager"></param>
    public ClaimTransformation(IHostEnvironment environment, UserInRoleManager manager)
    {
        this.environment = environment;
        this.manager = manager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity != null && principal.Identity.IsAuthenticated)
        {
            if (principal.Identity is not ClaimsIdentity identity)
                throw new InvalidOperationException("无法处理Identity。Principal.Identity不是ClaimsIdentity。");

            var roleSet = new HashSet<string>();

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst(JwtClaimTypes.Subject);
            if (userIdClaim != null)
            {
                roleSet.UnionWith(this.manager.GetRoles(userIdClaim.Value));
            }

            //for debugging. always add Administrators role to user.
            if (this.environment.IsDevelopment())
            {
                roleSet.Add(RoleConstants.AdministratorsRole.Name);
            }

            identity.AddClaims(roleSet.Select(p => new Claim(identity.RoleClaimType, p)));
        }
        return Task.FromResult(principal);
    }
}
