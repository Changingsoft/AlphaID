using IDSubjects;

namespace AlphaID.EntityFramework;

public class OrganizationStore : IOrganizationStore
{
    private readonly IdSubjectsDbContext dbContext;

    public OrganizationStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<GenericOrganization> Organizations => this.dbContext.Organizations;

    public async Task<IdOperationResult> CreateAsync(GenericOrganization organization)
    {
        this.dbContext.Organizations.Add(organization);
        _ = await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(GenericOrganization organization)
    {
        this.dbContext.Organizations.Remove(organization);
        _ = await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<GenericOrganization?> FindByIdAsync(string id)
    {
        return await this.dbContext.Organizations.FindAsync(id);
    }

    public async Task<IdOperationResult> UpdateAsync(GenericOrganization organization)
    {
        this.dbContext.Entry(organization).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
