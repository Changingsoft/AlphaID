using IdSubjects;
using IdSubjects.Invitations;

namespace AlphaId.EntityFramework;
public class JoinOrganizationInvitationStore : IJoinOrganizationInvitationStore
{
    private readonly IdSubjectsDbContext dbContext;

    public JoinOrganizationInvitationStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<JoinOrganizationInvitation> Invitations => this.dbContext.JoinOrganizationInvitations;
    public async Task<IdOperationResult> CreateAsync(JoinOrganizationInvitation invitation)
    {
        this.dbContext.JoinOrganizationInvitations.Add(invitation);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(JoinOrganizationInvitation invitation)
    {
        //todo 是否要考虑attach或者其他操作以避免外来对象无法正确更新到数据库。
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(JoinOrganizationInvitation invitation)
    {
        this.dbContext.JoinOrganizationInvitations.Remove(invitation);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
