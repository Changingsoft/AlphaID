using IDSubjects;

namespace AlphaIDEntityFramework.EntityFramework;

public class OrganizationMemberStore : IOrganizationMemberStore
{
    private readonly IDSubjectsDbContext dbContext;

    public OrganizationMemberStore(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<OrganizationMember> OrganizationMembers => this.dbContext.OrganizationMembers;

    public async Task CreateAsync(OrganizationMember item)
    {
        this.dbContext.OrganizationMembers.Add(item);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrganizationMember item)
    {
        this.dbContext.OrganizationMembers.Remove(item);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrganizationMember item)
    {
        this.dbContext.OrganizationMembers.Update(item);
        await this.dbContext.SaveChangesAsync();
    }
}
