using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthCenterWebApp.Services;

/// <summary>
/// 不起任何作用的RoleStore.
/// </summary>
internal class FakeRoleStore :
    IQueryableRoleStore<IdentityRole>,
    IRoleClaimStore<IdentityRole>
{
    public IQueryable<IdentityRole> Roles => throw new NotImplementedException();

    public Task AddClaimAsync(IdentityRole role, Claim claim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Claim>> GetClaimsAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveClaimAsync(IdentityRole role, Claim claim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
