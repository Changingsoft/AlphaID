using IdSubjects;
using IdSubjects.Invitations;

namespace AlphaId.EntityFramework;

public class JoinOrganizationInvitationStore(IdSubjectsDbContext dbContext) : IJoinOrganizationInvitationStore
{
    public IQueryable<JoinOrganizationInvitation> Invitations => dbContext.JoinOrganizationInvitations;

    public async Task<IdOperationResult> CreateAsync(JoinOrganizationInvitation invitation)
    {
        dbContext.JoinOrganizationInvitations.Add(invitation);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(JoinOrganizationInvitation invitation)
    {
        dbContext.JoinOrganizationInvitations.Update(invitation);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(JoinOrganizationInvitation invitation)
    {
        dbContext.JoinOrganizationInvitations.Remove(invitation);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}