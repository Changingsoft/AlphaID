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
    /// Get the member.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="organization"></param>
    /// <returns></returns>
    public Task<OrganizationMember?> GetMemberAsync(string person, string organization)
    {
        OrganizationMember? result =
            store.OrganizationMembers.FirstOrDefault(
                p => p.PersonId == person && p.OrganizationId == organization);
        return Task.FromResult(result);
    }

    /// <summary>
    /// 获取可见的组织成员。
    /// </summary>
    /// <remarks>
    /// 该方法会考虑组织成员的Visibility设置，根据visitor来决定是否在结果集合中包括这些成员。
    /// </remarks>
    /// <param name="organization">AN organization that members to get.</param>
    /// <param name="visitor">The person who access this system. null if anonymous access.</param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetVisibleMembersAsync(Organization organization,
        NaturalPerson? visitor)
    {
        IQueryable<OrganizationMember>? members =
            store.OrganizationMembers.Where(p => p.OrganizationId == organization.Id);
        Debug.Assert(members != null);
        //假定可见级别为Public。
        var visibilityLevel = MembershipVisibility.Public;
        if (visitor != null)
        {
            //如果已登录，降为AuthenticatedUser
            visibilityLevel = MembershipVisibility.AuthenticatedUser;
            //如果访问者是该组织成员，则降为Private
            if (members.Any(m => m.PersonId == visitor.Id))
                visibilityLevel = MembershipVisibility.Private; //Visitor is a member of the organization.
        }
        // 过滤出成员可见级别大于等于访问者最低可见级别的成员。
        return Task.FromResult(members.Where(m => m.Visibility >= visibilityLevel).AsEnumerable());
    }

    /// <summary>
    /// Get organization members.
    /// </summary>
    /// <param name="organization">Organization</param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetMembersAsync(Organization organization)
    {
        IQueryable<OrganizationMember> members =
            store.OrganizationMembers.Where(p => p.OrganizationId == organization.Id);
        return Task.FromResult(members.AsEnumerable());
    }

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
    /// 获取个人的组织成员身份。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetMembersOfAsync(NaturalPerson person)
    {
        IQueryable<OrganizationMember> members = store.OrganizationMembers.Where(p => p.PersonId == person.Id);
        return Task.FromResult(members.AsEnumerable());
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
    /// Take person leave out the organization.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> LeaveOrganizationAsync(OrganizationMember member)
    {
        IQueryable<OrganizationMember> members =
            store.OrganizationMembers.Where(m => m.OrganizationId == member.OrganizationId);

        if (member.IsOwner && members.Count(m => m.IsOwner) <= 1)
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
        var members = await GetMembersAsync(member.Organization);
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
        var members = await GetMembersAsync(member.Organization);
        if (members.Count(m => m.IsOwner) > 1)
        {
            member.IsOwner = false;
            //todo need log to audit log.
            return await UpdateAsync(member);
        }
        return OrganizationOperationResult.Failed(string.Format(Resources.Max_owners_in_the_organization, 5));
    }
}