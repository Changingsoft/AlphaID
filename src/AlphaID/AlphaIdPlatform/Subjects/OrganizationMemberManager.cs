using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// Organization Member Manager.
/// </summary>
/// <remarks>
/// Init Organization Member Manager via Organization Member store.
/// </remarks>
/// <param name="store"></param>
/// <param name="organizationManager"></param>
public class OrganizationMemberManager(IOrganizationMemberStore store, OrganizationManager organizationManager)
{
    /// <summary>
    /// 以访问者visitor的视角检索指定用户的组织成员身份。
    /// </summary>
    /// <param name="personId">要检索组织成员身份的目标用户。</param>
    /// <param name="visitorId">访问者。如果传入null，代表匿名访问者。</param>
    /// <returns></returns>
    [Obsolete]
    public IQueryable<OrganizationMember> GetVisibleMembersOf(string personId, string? visitorId)
    {
        //获取目标person的所有组织身份。
        IQueryable<OrganizationMember>? members = store.OrganizationMembers.Where(p => p.PersonId == personId);
        Debug.Assert(members != null);

        if (visitorId == null)
            return members.Where(m => m.Visibility == MembershipVisibility.Public);

        //获取访问者的所属组织Id列表。
        List<string> visitorMemberOfOrgIds = [.. store.OrganizationMembers.Where(m => m.PersonId == visitorId).Select(m => m.OrganizationId)];

        return members.Where(m =>
            m.Visibility >= MembershipVisibility.AuthenticatedUser || m.Visibility == MembershipVisibility.Private &&
                                                                       visitorMemberOfOrgIds
                                                                           .Contains(m.OrganizationId));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="userId"></param>
    /// <param name="visibility"></param>
    /// <returns></returns>
    public async Task<OrganizationMember> Join(string organizationId, string userId, MembershipVisibility visibility)
    {
        if (store.OrganizationMembers.Any(p =>
                p.OrganizationId == organizationId && p.PersonId == userId))
            throw new InvalidOperationException(Resources.MembershipExists);
        var organization = await organizationManager.FindByIdAsync(organizationId) ?? throw new ArgumentException(Resources.OrganizationNotFound, nameof(organizationId));
        var member = new OrganizationMember(organization, userId, visibility);
        var result = await store.CreateAsync(member);
        if (!result.Succeeded)
            throw new InvalidOperationException(Resources.MembershipCreateFailed);
        return member;
    }

    /// <summary>
    /// 从指定组织中移除成员。
    /// </summary>
    /// <remarks>如果为设置force为true，当用户是组织的唯一所有者时，不能离开组织。这种情况下，操作将失败，并且结果将包含一条错误消息，指示最后一位所有者无法离开。</remarks>
    /// <param name="organizationId">要从中移除用户的组织唯一标识符。</param>
    /// <param name="userId">要从组织中移除的用户唯一标识符。</param>
    /// <param name="force">强制离开，默认为false。如果设为true，即使该成员为组织的最后1个所有者，也将其离开组织。</param>
    /// <returns>一个 <see cref="OrganizationOperationResult"/>，指示操作结果。如果用户已成功移除或本就不是成员，则返回 <see cref="OrganizationOperationResult.Success"/>。如果操作无法完成，则返回带有相应错误消息的 <see cref="OrganizationOperationResult.Failed"/>。</returns>
    public async Task<OrganizationOperationResult> Leave(string organizationId, string userId, bool force = false)
    {
        var orgMembers = store.OrganizationMembers.Where(m => m.OrganizationId == organizationId);
        var member =
            orgMembers.FirstOrDefault(p => p.PersonId == userId);
        if (member == null)
            return OrganizationOperationResult.Failed(Resources.OrganizationMemberNotFound);

        if (force)
            return await store.DeleteAsync(member);

        // 如果用户是组织所有者，并且该组织只有一个所有者，则不能离开组织。
        if (member.IsOwner && orgMembers.Count(m => m.OrganizationId == organizationId && m.IsOwner) <= 1)
            return OrganizationOperationResult.Failed(Resources.LastOwnerCannotLeave);

        return await store.DeleteAsync(member);
    }

    /// <summary>
    /// Update organization member info.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task<OrganizationOperationResult> UpdateAsync(OrganizationMember member)
    {
        return store.UpdateAsync(member);
    }
}