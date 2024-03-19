using IdSubjects;
using IdSubjects.Invitations;
using Microsoft.EntityFrameworkCore;

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
        if (dbContext.Entry(invitation).State == EntityState.Detached)
        {
            var origin = await dbContext.JoinOrganizationInvitations.FindAsync(invitation.Id);
            if (origin != null)
            {
                dbContext.Entry(origin).CurrentValues.SetValues(invitation);
            }
        }
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
