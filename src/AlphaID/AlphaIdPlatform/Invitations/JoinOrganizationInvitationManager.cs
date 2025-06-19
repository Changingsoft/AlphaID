using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlphaIdPlatform.Invitations;

/// <summary>
/// 加入组织邀请管理器。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="store"></param>
/// <param name="personManager"></param>
/// <param name="organizationManager"></param>
public class JoinOrganizationInvitationManager(
    IJoinOrganizationInvitationStore store,
    UserManager<NaturalPerson> personManager,
    OrganizationManager organizationManager)
{
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 获取我收到的邀请。
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<JoinOrganizationInvitation> GetPendingInvitations(string personId)
    {
        return store.Invitations.Where(i =>
            i.InviteeId == personId && !i.Accepted.HasValue && i.WhenExpired > TimeProvider.GetLocalNow());
    }

    /// <summary>
    /// 获取组织发出的邀请
    /// </summary>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<JoinOrganizationInvitation> GetIssuedInvitations(string organizationId)
    {
        return store.Invitations.Where(i => i.OrganizationId == organizationId);
    }

    /// <summary>
    /// 邀请某人加入到组织。
    /// </summary>
    /// <param name="organization">要加入的组织。</param>
    /// <param name="invitee">邀请人的用户名</param>
    /// <param name="inviter"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> InviteMemberAsync(Organization organization,
        NaturalPerson invitee,
        string inviter)
    {
        var errors = new List<string>();
        if (organization.Members.Any(m => m.PersonId == invitee.Id))
            errors.Add("Person is a member of this organization.");

        if (store.Invitations.Any(i =>
                i.InviteeId == invitee.Id && i.OrganizationId == organization.Id &&
                i.WhenExpired > TimeProvider.GetUtcNow()))
            errors.Add("You've been sent invitation to this person.");
        if (errors.Count != 0)
            return OrganizationOperationResult.Failed([.. errors]);

        return await store.CreateAsync(new JoinOrganizationInvitation
        {
            Inviter = inviter,
            OrganizationId = organization.Id,
            InviteeId = invitee.Id,
            WhenCreated = DateTimeOffset.UtcNow,
            WhenExpired = DateTimeOffset.UtcNow.AddDays(1.0f), //todo 应可以在设置中更改。
            ExpectVisibility = MembershipVisibility.Public //todo 邀请时应可填写期望的可见性。
        });
    }

    /// <summary>
    /// 接受邀请并加入组织。
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> AcceptAsync(JoinOrganizationInvitation invitation)
    {
        if (invitation.Accepted.HasValue)
            return OrganizationOperationResult.Failed("Invitation has been processed.");
        var org = await organizationManager.FindByIdAsync(invitation.OrganizationId);
        if (org == null)
            return OrganizationOperationResult.Failed("Organization not found.");
        if (org.Members.Any(m => m.PersonId == invitation.InviteeId))
            return OrganizationOperationResult.Failed("用户已在组织中");

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        NaturalPerson? person = await personManager.FindByIdAsync(invitation.InviteeId);
        Organization? organization = await organizationManager.FindByIdAsync(invitation.OrganizationId);
        OrganizationOperationResult? result = null;
        if (organization != null && person != null)
        {
            var newMember = new OrganizationMember(organization, person.Id, invitation.ExpectVisibility);
            organization.Members.Add(newMember);
            await organizationManager.UpdateAsync(organization);
        }

        invitation.Accepted = true;
        await store.UpdateAsync(invitation);
        trans.Complete();

        return result ?? OrganizationOperationResult.Success;
    }

    /// <summary>
    /// 拒绝邀请。
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> RefuseAsync(JoinOrganizationInvitation invitation)
    {
        if (invitation.Accepted.HasValue)
            return OrganizationOperationResult.Failed("Invitation has been processed.");

        invitation.Accepted = false;
        return await store.UpdateAsync(invitation);
    }

    /// <summary>
    /// Find invitation by id.
    /// </summary>
    /// <param name="invitationId"></param>
    /// <returns></returns>
    public ValueTask<JoinOrganizationInvitation?> FindById(int invitationId)
    {
        return ValueTask.FromResult(store.Invitations.FirstOrDefault(i => i.Id == invitationId));
    }

    /// <summary>
    /// Revoke invitation.
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    public Task<OrganizationOperationResult> Revoke(JoinOrganizationInvitation invitation)
    {
        return store.DeleteAsync(invitation);
    }
}