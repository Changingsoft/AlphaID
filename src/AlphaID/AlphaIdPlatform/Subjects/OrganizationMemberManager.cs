using IdSubjects;
using System.Diagnostics;
using AlphaIdPlatform.Identity;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// Organization Member Manager.
/// </summary>
/// <remarks>
/// Init Organization Member Manager via Organization Member store.
/// </remarks>
/// <param name="store"></param>
public class OrganizationMemberManager(IOrganizationMemberStore store)
{
    /// <summary>
    /// 以访问者visitor的视角检索指定用户的组织成员身份。
    /// </summary>
    /// <param name="personId">要检索组织成员身份的目标用户。</param>
    /// <param name="visitorId">访问者。如果传入null，代表匿名访问者。</param>
    /// <returns></returns>
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
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> CreateAsync(OrganizationMember member)
    {
        if (store.OrganizationMembers.Any(p =>
                p.OrganizationId == member.OrganizationId && p.PersonId == member.PersonId))
            return OrganizationOperationResult.Failed(Resources.Membership_exists);
        return await store.CreateAsync(member);
    }

    /// <summary>
    /// 从指定组织中移除用户。
    /// </summary>
    /// <remarks>如果用户是组织的唯一所有者，则不能离开组织。在这种情况下，操作将失败，并且结果将包含一条错误消息，指示最后一位所有者无法离开。</remarks>
    /// <param name="organizationId">要从中移除用户的组织唯一标识符。</param>
    /// <param name="userId">要从组织中移除的用户唯一标识符。</param>
    /// <returns>一个 <see cref="OrganizationOperationResult"/>，指示操作结果。如果用户已成功移除或本就不是成员，则返回 <see cref="OrganizationOperationResult.Success"/>。如果操作无法完成，则返回带有相应错误消息的 <see cref="OrganizationOperationResult.Failed"/>。</returns>
    public async Task<OrganizationOperationResult> LeaveUser(string organizationId, string userId)
    {
        var member =
            store.OrganizationMembers.FirstOrDefault(p => p.OrganizationId == organizationId && p.PersonId == userId);
        if (member == null)
            return OrganizationOperationResult.Success;

        // 如果用户是组织所有者，并且该组织只有一个所有者，则不能离开组织。
        if (member.IsOwner && store.OrganizationMembers.Count(m => m.OrganizationId == organizationId && m.IsOwner) <= 1)
            return OrganizationOperationResult.Failed(Resources.LastOwnerCannotLeave);

        return await store.DeleteAsync(member);
    }

    /// <summary>
    /// 移除用户成员身份，无论用户是否是组织所有者也是如此。
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task<OrganizationOperationResult> RemoveAsync(OrganizationMember member)
    {
        return store.DeleteAsync(member);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> SetOwner(OrganizationMember member)
    {
        var members = store.OrganizationMembers.Where(m => m.OrganizationId == member.OrganizationId);
        if (members.Count(m => m.IsOwner) <= 5)
        {
            member.IsOwner = true;
            //todo need log to audit log.
            return await UpdateAsync(member);
        }
        return OrganizationOperationResult.Failed(string.Format(Resources.Max_owners_in_the_organization, 5));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> UnsetOwner(OrganizationMember member)
    {
        var members = store.OrganizationMembers.Where(m => m.OrganizationId == member.OrganizationId);
        if (members.Count(m => m.IsOwner) > 1)
        {
            member.IsOwner = false;
            //todo need log to audit log.
            return await UpdateAsync(member);
        }
        return OrganizationOperationResult.Failed(string.Format(Resources.Max_owners_in_the_organization, 5));
    }
}