using AlphaIdPlatform.Invitations;
using Organizational;

namespace AlphaId.EntityFramework.IdSubjects;

public class JoinOrganizationInvitationStore(AlphaIdDbContext dbContext) : IJoinOrganizationInvitationStore
{
    public IQueryable<JoinOrganizationInvitation> Invitations => dbContext.JoinOrganizationInvitations;

    public async Task<OrganizationOperationResult> CreateAsync(JoinOrganizationInvitation invitation)
    {
        dbContext.JoinOrganizationInvitations.Add(invitation);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }

    public async Task<OrganizationOperationResult> UpdateAsync(JoinOrganizationInvitation invitation)
    {
        dbContext.JoinOrganizationInvitations.Update(invitation);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }

    public async Task<OrganizationOperationResult> DeleteAsync(JoinOrganizationInvitation invitation)
    {
        dbContext.JoinOrganizationInvitations.Remove(invitation);
        await dbContext.SaveChangesAsync();
        return OrganizationOperationResult.Success;
    }
}