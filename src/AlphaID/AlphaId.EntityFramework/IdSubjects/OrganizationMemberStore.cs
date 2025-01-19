using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

internal class OrganizationMemberStore(IdSubjectsDbContext dbContext) : IOrganizationMemberStore
{
    public IQueryable<OrganizationMember> OrganizationMembers =>
        dbContext.OrganizationMembers.Include(p => p.Organization).Include(p => p.Person);

    public Task<OrganizationMember?> FindAsync(string personId, string organizationId)
    {
        return dbContext.OrganizationMembers.FirstOrDefaultAsync(p =>
            p.PersonId == personId && p.OrganizationId == organizationId);
    }

    public async Task<OrganizationOperationResult> CreateAsync(OrganizationMember item)
    {
        dbContext.OrganizationMembers.Add(item);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }

    public async Task<OrganizationOperationResult> DeleteAsync(OrganizationMember item)
    {
        dbContext.OrganizationMembers.Remove(item);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }

    public async Task<OrganizationOperationResult> UpdateAsync(OrganizationMember item)
    {
        dbContext.OrganizationMembers.Update(item);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }
}