using System.Diagnostics;

namespace IdSubjects;

/// <summary>
///     GenericOrganization Member Manager.
/// </summary>
/// <remarks>
///     Init GenericOrganization Member Manager via GenericOrganization Member store.
/// </remarks>
/// <param name="store"></param>
public class OrganizationMemberManager(IOrganizationMemberStore store)
{
    /// <summary>
    ///     Get the member.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="organization"></param>
    /// <returns></returns>
    public Task<OrganizationMember?> GetMemberAsync(NaturalPerson person, GenericOrganization organization)
    {
        OrganizationMember? result =
            store.OrganizationMembers.FirstOrDefault(
                p => p.PersonId == person.Id && p.OrganizationId == organization.Id);
        return Task.FromResult(result);
    }

    /// <summary>
    ///     Get members of the organization.
    /// </summary>
    /// <param name="organization">AN organization that members to get.</param>
    /// <param name="visitor">The person who access this system. null if anonymous access.</param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetVisibleMembersAsync(GenericOrganization organization,
        NaturalPerson? visitor)
    {
        IQueryable<OrganizationMember>? members =
            store.OrganizationMembers.Where(p => p.OrganizationId == organization.Id);
        Debug.Assert(members != null);
        var visibilityLevel = MembershipVisibility.Public;
        if (visitor != null)
        {
            visibilityLevel = MembershipVisibility.AuthenticatedUser;
            if (members.Any(m => m.PersonId == visitor.Id))
                visibilityLevel = MembershipVisibility.Private; //Visitor is a member of the organization.
        }

        return Task.FromResult(members.Where(m => m.Visibility >= visibilityLevel).AsEnumerable());
    }

    /// <summary>
    ///     Get organization members.
    /// </summary>
    /// <param name="organization">Organization</param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetMembersAsync(GenericOrganization organization)
    {
        IQueryable<OrganizationMember> members =
            store.OrganizationMembers.Where(p => p.OrganizationId == organization.Id);
        return Task.FromResult(members.AsEnumerable());
    }

    /// <summary>
    ///     以访问者visitor的视角检索指定用户的组织成员身份。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         如果
    ///     </para>
    /// </remarks>
    /// <param name="person">要检索组织成员身份的目标用户。</param>
    /// <param name="visitor">访问者。如果传入null，代表匿名访问者。</param>
    /// <returns></returns>
    public IEnumerable<OrganizationMember> GetVisibleMembersOf(NaturalPerson person, NaturalPerson? visitor)
    {
        //获取目标person的所有组织身份。
        IQueryable<OrganizationMember>? members = store.OrganizationMembers.Where(p => p.PersonId == person.Id);
        Debug.Assert(members != null);

        if (visitor == null)
            return members.Where(m => m.Visibility == MembershipVisibility.Public);

        //获取访问者的所属组织Id列表。
        List<string> visitorMemberOfOrgIds = store.OrganizationMembers.Where(m => m.PersonId == visitor.Id)
            .Select(m => m.OrganizationId).ToList();

        return members.Where(m =>
            m.Visibility >= MembershipVisibility.AuthenticatedUser || (m.Visibility == MembershipVisibility.Private &&
                                                                       visitorMemberOfOrgIds
                                                                           .Contains(m.OrganizationId)));
    }

    /// <summary>
    ///     获取个人的组织成员身份。
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
    public async Task<IdOperationResult> CreateAsync(OrganizationMember member)
    {
        if (store.OrganizationMembers.Any(p =>
                p.OrganizationId == member.OrganizationId && p.PersonId == member.PersonId))
            return IdOperationResult.Failed(Resources.Membership_exists);
        await store.CreateAsync(member);
        return IdOperationResult.Success;
    }

    /// <summary>
    ///     Take person leave out the organization.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> LeaveOrganizationAsync(OrganizationMember member)
    {
        IQueryable<OrganizationMember> members =
            store.OrganizationMembers.Where(m => m.OrganizationId == member.OrganizationId);

        if (member.IsOwner && members.Count(m => m.IsOwner) <= 1)
            return IdOperationResult.Failed(Resources.LastOwnerCannotLeave);

        return await store.DeleteAsync(member);
    }

    /// <summary>
    ///     移除用户成员身份，无论用户是否是组织所有者也是如此。
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task<IdOperationResult> RemoveAsync(OrganizationMember member)
    {
        return store.DeleteAsync(member);
    }

    /// <summary>
    ///     Update organization member info.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task<IdOperationResult> UpdateAsync(OrganizationMember member)
    {
        return store.UpdateAsync(member);
    }
}