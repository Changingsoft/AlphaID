using IDSubjects;

namespace AlphaID.EntityFramework;

public class OrganizationMemberStore : IOrganizationMemberStore
{
    private readonly IDSubjectsDbContext dbContext;

    public OrganizationMemberStore(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<OrganizationMember> OrganizationMembers => this.dbContext.OrganizationMembers;

    public async Task<IdOperationResult> CreateAsync(OrganizationMember item)
    {
        this.dbContext.OrganizationMembers.Add(item);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(OrganizationMember item)
    {
        this.dbContext.OrganizationMembers.Remove(item);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(OrganizationMember item)
    {
        this.dbContext.OrganizationMembers.Update(item);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
