using IdSubjects;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework;

internal class OrganizationStore(IdSubjectsDbContext dbContext) : IOrganizationStore
{
    public IQueryable<GenericOrganization> Organizations => dbContext.Organizations.AsNoTracking();

    public IEnumerable<GenericOrganization> FindByName(string name)
    {
        return dbContext.Organizations.Where(o => o.Name == name).Take(10); //todo 返回条目过多可能导致性能问题。
    }

    public async Task<IdOperationResult> CreateAsync(GenericOrganization organization)
    {
        dbContext.Organizations.Add(organization);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(GenericOrganization organization)
    {
        dbContext.Organizations.Remove(organization);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<GenericOrganization?> FindByIdAsync(string id)
    {
        return await dbContext.Organizations.FindAsync(id);
    }

    public GenericOrganization? FindById(string id)
    {
        return dbContext.Organizations.Find(id);
    }

    public async Task<IdOperationResult> UpdateAsync(GenericOrganization organization)
    {
        dbContext.Organizations.Update(organization);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}