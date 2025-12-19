using Organizational;

namespace AlphaId.EntityFramework.IdSubjects;

internal class OrganizationStore(AlphaIdDbContext dbContext) : IOrganizationStore
{
    public IQueryable<Organization> Organizations => dbContext.Organizations;

    public async Task<OrganizationOperationResult> CreateAsync(Organization organization)
    {
        dbContext.Organizations.Add(organization);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }

    public async Task<OrganizationOperationResult> DeleteAsync(Organization organization)
    {
        dbContext.Organizations.Remove(organization);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }

    public async Task<Organization?> FindByIdAsync(string id)
    {
        return await dbContext.Organizations.FindAsync(id);
    }

    public async Task<OrganizationOperationResult> UpdateAsync(Organization organization)
    {
        dbContext.Organizations.Update(organization);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }
}