using IDSubjects;

namespace AlphaIDEntityFramework.EntityFramework;

public class OrganizationStore : IOrganizationStore
{
    private readonly IDSubjectsDbContext dbContext;

    public OrganizationStore(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<GenericOrganization> Organizations => this.dbContext.Organizations;

    public async Task CreateAsync(GenericOrganization organization)
    {
        this.dbContext.Organizations.Add(organization);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(GenericOrganization organization)
    {
        this.dbContext.Organizations.Remove(organization);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<GenericOrganization?> FindByIdAsync(string id)
    {
        return await this.dbContext.Organizations.FindAsync(id);
    }

    public Task<GenericOrganization?> FindByIdentityAsync(string identityType, string identityValue)
    {
        if (string.IsNullOrWhiteSpace(identityType) || string.IsNullOrWhiteSpace(identityValue))
            return Task.FromResult(default(GenericOrganization));

        identityType = identityType.Trim();
        identityValue = identityValue.Trim();
        var organization = this.dbContext.Organizations.FirstOrDefault(p => p.USCI == identityValue);

        return Task.FromResult(organization);
    }

    public async Task UpdateAsync(GenericOrganization organization)
    {
        this.dbContext.Entry(organization).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
    }
}
