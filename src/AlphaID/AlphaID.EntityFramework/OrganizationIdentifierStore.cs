using IdSubjects;

namespace AlphaId.EntityFramework;

internal class OrganizationIdentifierStore(IdSubjectsDbContext dbContext) : IOrganizationIdentifierStore
{
    public IQueryable<OrganizationIdentifier> Identifiers => dbContext.OrganizationIdentifiers;

    public async Task<IdOperationResult> CreateAsync(OrganizationIdentifier identifier)
    {
        dbContext.OrganizationIdentifiers.Add(identifier);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(OrganizationIdentifier identifier)
    {
        dbContext.OrganizationIdentifiers.Update(identifier);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(OrganizationIdentifier identifier)
    {
        dbContext.OrganizationIdentifiers.Remove(identifier);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}