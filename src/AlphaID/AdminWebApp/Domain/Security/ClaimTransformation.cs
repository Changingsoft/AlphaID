using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;

namespace AdminWebApp.Domain.Security;

/// <summary>
///     Claim transformation for role-based claims.
/// </summary>
/// <remarks>
///     Initialize.
/// </remarks>
/// <param name="environment"></param>
/// <param name="manager"></param>
public class ClaimTransformation(IHostEnvironment environment, UserInRoleManager manager) : IClaimsTransformation
{
    /// <summary>
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not { IsAuthenticated: true }) return Task.FromResult(principal);

        if (principal.Identity is not ClaimsIdentity identity)
            throw new InvalidOperationException("无法处理Identity。Principal.Identity不是ClaimsIdentity。");

        var roleSet = new HashSet<string>();

        Claim? userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ??
                             principal.FindFirst(JwtClaimTypes.Subject);
        if (userIdClaim != null) roleSet.UnionWith(manager.GetRoles(userIdClaim.Value));

        //for debugging. always add Administrators role to user.
        if (environment.IsDevelopment()) roleSet.Add(RoleConstants.AdministratorsRole.Name);

        identity.AddClaims(roleSet.Select(p => new Claim(identity.RoleClaimType, p)));
        return Task.FromResult(principal);
    }
}