using AlphaIdPlatform.Subjects;

namespace AlphaIdPlatform.Tests;
internal class StubOrganizationStore : IOrganizationStore
{
    private readonly List<Organization> _organizations = new();

    public IQueryable<Organization> Organizations => _organizations.AsQueryable();

    public Task<Organization?> FindByIdAsync(string id)
    {
        var organization = _organizations.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(organization);
    }

    public Task<OrganizationOperationResult> CreateAsync(Organization organization)
    {
        _organizations.Add(organization);
        return Task.FromResult(OrganizationOperationResult.Success);
    }

    public Task<OrganizationOperationResult> UpdateAsync(Organization organization)
    {
        var existingOrganization = _organizations.FirstOrDefault(o => o.Id == organization.Id);
        if (existingOrganization == null)
        {
            return Task.FromResult(OrganizationOperationResult.Failed("Organization not found"));
        }

        _organizations.Remove(existingOrganization);
        _organizations.Add(organization);
        return Task.FromResult(OrganizationOperationResult.Success);
    }

    public Task<OrganizationOperationResult> DeleteAsync(Organization organization)
    {
        var existingOrganization = _organizations.FirstOrDefault(o => o.Id == organization.Id);
        if (existingOrganization == null)
        {
            return Task.FromResult(OrganizationOperationResult.Failed("Organization not found"));
        }

        _organizations.Remove(existingOrganization);
        return Task.FromResult(OrganizationOperationResult.Success);
    }
}
