using IdSubjects;

namespace AlphaId.EntityFramework;

internal class OrganizationIdentifierStore : IOrganizationIdentifierStore
{
    private readonly IdSubjectsDbContext dbContext;

    public OrganizationIdentifierStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<OrganizationIdentifier> Identifiers => this.dbContext.OrganizationIdentifiers;
    public async Task<IdOperationResult> CreateAsync(OrganizationIdentifier identifier)
    {
        this.dbContext.OrganizationIdentifiers.Add(identifier);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(OrganizationIdentifier identifier)
    {
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(OrganizationIdentifier identifier)
    {
        this.dbContext.OrganizationIdentifiers.Remove(identifier);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
