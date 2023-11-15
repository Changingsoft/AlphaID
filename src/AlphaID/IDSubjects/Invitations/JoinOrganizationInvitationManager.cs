using System.Transactions;

namespace IdSubjects.Invitations;

/// <summary>
/// 加入组织邀请管理器。
/// </summary>
public class JoinOrganizationInvitationManager
{
    private readonly IJoinOrganizationInvitationStore store;
    private readonly OrganizationMemberManager memberManager;

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    /// <param name="memberManager"></param>
    public JoinOrganizationInvitationManager(IJoinOrganizationInvitationStore store, OrganizationMemberManager memberManager)
    {
        this.store = store;
        this.memberManager = memberManager;
    }

    /// <summary>
    /// 获取我收到的邀请。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<JoinOrganizationInvitation> GetPendingInvitations(NaturalPerson person)
    {
        return this.store.Invitations.Where(i => i.InviteeId == person.Id && !i.Accepted.HasValue && i.WhenExpired > this.TimeProvider.GetLocalNow());
    }

    /// <summary>
    /// 获取组织发出的邀请
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<JoinOrganizationInvitation> GetIssuedInvitations(GenericOrganization organization)
    {
        return this.store.Invitations.Where(i => i.OrganizationId == organization.Id);
    }

    /// <summary>
    /// 邀请某人加入到组织。
    /// </summary>
    /// <param name="organization">要加入的组织。</param>
    /// <param name="invitee">邀请人的用户名</param>
    /// <param name="inviter"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> InviteMemberAsync(GenericOrganization organization, NaturalPerson invitee,
        string inviter)
    {
        var errors = new List<string>();
        var existsMember = await this.memberManager.GetMemberAsync(invitee, organization);
        if (existsMember != null)
            errors.Add("Person is a member of this organization.");

        if (this.store.Invitations.Any(i => i.InviteeId == invitee.Id && i.OrganizationId == organization.Id && i.WhenExpired > this.TimeProvider.GetUtcNow()))
            errors.Add("You've been sent invitation to this person.");
        if (errors.Any())
            return IdOperationResult.Failed(errors.ToArray());

        return await this.store.CreateAsync(new JoinOrganizationInvitation()
        {
            Inviter = inviter,
            OrganizationId = organization.Id,
            Organization = organization,
            InviteeId = invitee.Id,
            Invitee = invitee,
        });
    }

    /// <summary>
    /// 接受邀请并加入组织。
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AcceptAsync(JoinOrganizationInvitation invitation)
    {
        if (invitation.Accepted.HasValue)
            return IdOperationResult.Failed("Invitation has been processed.");

        var existedMember = await this.memberManager.GetMemberAsync(invitation.Invitee, invitation.Organization);
        if (existedMember != null) return IdOperationResult.Failed("Person is already a member.");

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var result = await this.memberManager.CreateAsync(new OrganizationMember(invitation.Organization, invitation.Invitee) { Visibility = invitation.ExpectVisibility });
        if (!result.Succeeded)
            return result;
        invitation.Accepted = true;
        result = await this.store.UpdateAsync(invitation);
        if (!result.Succeeded)
            return result;

        //所有事务性操作都成功，提交事务。
        trans.Complete();
        return result;
    }

    /// <summary>
    /// 拒绝邀请。
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> RefuseAsync(JoinOrganizationInvitation invitation)
    {
        if (invitation.Accepted.HasValue)
            return IdOperationResult.Failed("Invitation has been processed.");

        invitation.Accepted = false;
        return await this.store.UpdateAsync(invitation);
    }


}