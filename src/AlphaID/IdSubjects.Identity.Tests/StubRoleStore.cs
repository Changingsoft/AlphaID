using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Tests;
internal class StubRoleStore : IRoleStore<IdentityRole>
{
    private readonly List<IdentityRole> _roles = [];

    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        _roles.Add(role);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        var existingRole = _roles.FirstOrDefault(r => r.Id == role.Id);
        if (existingRole != null)
        {
            existingRole.Name = role.Name;
            existingRole.NormalizedName = role.NormalizedName;
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        var existingRole = _roles.FirstOrDefault(r => r.Id == role.Id);
        if (existingRole != null)
        {
            _roles.Remove(existingRole);
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

    public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        return Task.FromResult(role);
    }

    public Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        var role = _roles.FirstOrDefault(r => r.NormalizedName == normalizedRoleName);
        return Task.FromResult(role);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
