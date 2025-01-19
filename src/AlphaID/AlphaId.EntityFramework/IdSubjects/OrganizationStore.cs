using AlphaIdPlatform.Subjects;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

internal class OrganizationStore(IdSubjectsDbContext dbContext) : IOrganizationStore
{
    public IQueryable<Organization> Organizations => dbContext.Organizations.AsNoTracking();

    public IEnumerable<Organization> FindByName(string name)
    {
        return dbContext.Organizations.Where(o => o.Name == name).Take(10); //todo 返回条目过多可能导致性能问题。
    }

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

    public Organization? FindById(string id)
    {
        return dbContext.Organizations.Find(id);
    }

    public async Task<OrganizationOperationResult> UpdateAsync(Organization organization)
    {
        dbContext.Organizations.Update(organization);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }
}