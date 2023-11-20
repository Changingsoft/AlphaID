using IdSubjects;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework;

internal class OrganizationStore : IOrganizationStore
{
    private readonly IdSubjectsDbContext dbContext;

    public OrganizationStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<GenericOrganization> Organizations => this.dbContext.Organizations.AsNoTracking();

    public IEnumerable<GenericOrganization> FindByName(string name)
    {
        return this.dbContext.Organizations.Where(o => o.Name == name).Take(10); //返回条目过多可能导致性能问题。
    }

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

    public GenericOrganization? FindById(string id)
    {
        return this.dbContext.Organizations.Find(id);
    }

    public async Task<IdOperationResult> UpdateAsync(GenericOrganization organization)
    {
        this.dbContext.Entry(organization).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
