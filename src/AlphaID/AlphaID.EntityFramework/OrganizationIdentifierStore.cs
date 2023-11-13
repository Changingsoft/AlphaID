using IDSubjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaID.EntityFramework;
public class OrganizationIdentifierStore : IOrganizationIdentifierStore
{
    private IdSubjectsDbContext dbContext;

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
