using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IDSubjects;

/// <summary>
/// GenericOrganization Member Manager.
/// </summary>
public class OrganizationMemberManager
{
    private readonly IOrganizationMemberStore store;
    private readonly ILogger<OrganizationMemberManager>? logger;

    /// <summary>
    /// Init GenericOrganization Member Manager via GenericOrganization Member store.
    /// </summary>
    /// <param name="store"></param>
    /// <param name="logger"></param>
    public OrganizationMemberManager(IOrganizationMemberStore store, ILogger<OrganizationMemberManager>? logger = null)
    {
        this.store = store;
        this.logger = logger;
    }

    /// <summary>
    /// Get the member.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="organization"></param>
    /// <returns></returns>
    public Task<OrganizationMember?> GetMemberAsync(NaturalPerson person, GenericOrganization organization)
    {
        var result = this.store.OrganizationMembers.FirstOrDefault(p => p.PersonId == person.Id && p.OrganizationId == organization.Id);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Get members of the organization.
    /// </summary>
    /// <param name="organization">A organization that members to get.</param>
    /// <param name="visitor">The person who access this system. null if anonymous access.</param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetVisibleMembersAsync(GenericOrganization organization, NaturalPerson? visitor)
    {
        var members = this.store.OrganizationMembers.Where(p => p.OrganizationId == organization.Id);
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
    /// Get organization members.
    /// </summary>
    /// <param name="organization">Organization</param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetMembersAsync(GenericOrganization organization)
    {
        var members = this.store.OrganizationMembers.Where(p => p.OrganizationId == organization.Id);
        return Task.FromResult(members.AsEnumerable());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="visitor">The person who access this system, null if anonymous access.</param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetVisibleMembersOfAsync(NaturalPerson person, NaturalPerson? visitor)
    {
        var members = this.store.OrganizationMembers.Where(p => p.PersonId == person.Id);
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
    /// 获取个人的组织成员身份。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetMembersOfAsync(NaturalPerson person)
    {
        var members = this.store.OrganizationMembers.Where(p => p.PersonId == person.Id);
        return Task.FromResult(members.AsEnumerable());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> CreateAsync(OrganizationMember member)
    {
        if (this.store.OrganizationMembers.Any(p => p.OrganizationId == member.OrganizationId && p.PersonId == member.PersonId))
            return IdOperationResult.Failed(Resources.MembershipExists);
        await this.store.CreateAsync(member);
        return IdOperationResult.Success;
    }

    /// <summary>
    /// Take person leave out the organization.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="organization"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> LeaveOrganizationAsync(NaturalPerson person, GenericOrganization organization)
    {
        var members = await this.GetMembersAsync(organization);
        var member = members.FirstOrDefault(m => m.PersonId == person.Id);
        if (member != null)
        {
            if (member.IsOwner && members.Count(m => m.IsOwner) <= 1)
                return IdOperationResult.Failed(Resources.LastOwnerCannotLeave);
            return await this.store.DeleteAsync(member);
        }
        return IdOperationResult.Success;
    }

    /// <summary>
    /// 移除用户成员身份，无论用户是否是组织所有者也是如此。
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> RemoveAsync(OrganizationMember member)
    {
        return await this.store.DeleteAsync(member);
    }

    /// <summary>
    /// Update organization member info.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> UpdateAsync(OrganizationMember member)
    {
        return await this.store.UpdateAsync(member);
    }
}
