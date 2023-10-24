using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IDSubjects;

/// <summary>
/// GenericOrganization Member Manager.
/// </summary>
public class OrganizationMemberManager
{
    private readonly IOrganizationMemberStore store;
    readonly ILogger<OrganizationMemberManager>? logger;

    /// <summary>
    /// Init GenericOrganization Member Mamager via GenericOrganization Member store.
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
        if(visitor != null)
        {
            visibilityLevel = MembershipVisibility.AuthenticatedUser;
            if (members.Any(m => m.PersonId == visitor.Id))
                visibilityLevel = MembershipVisibility.Private; //Visitor is a member of the organization.
        }
        return Task.FromResult(members.Where(m => m.Visibility >= visibilityLevel).AsEnumerable());
    }

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
        if(visitor != null)
        {
            visibilityLevel = MembershipVisibility.AuthenticatedUser;
            if(members.Any(m => m.PersonId == visitor.Id))
                visibilityLevel = MembershipVisibility.Private; //Visitor is a member of the organization.
        }
        return Task.FromResult(members.Where(m => m.Visibility >= visibilityLevel).AsEnumerable());
    }

    public Task<IEnumerable<OrganizationMember>> GetMembersOfAsync(NaturalPerson person)
    {
        var members = this.store.OrganizationMembers.Where(p => p.PersonId == person.Id);
        return Task.FromResult(members.AsEnumerable());
    }

    /// <summary>
    /// Take person join in organization.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="organization"></param>
    /// <param name="title"></param>
    /// <param name="department"></param>
    /// <param name="remark"></param>
    /// <returns></returns>
    public async Task<OperationResult> JoinOrganizationAsync(NaturalPerson person, GenericOrganization organization, string? title = null, string? department = null, string? remark = null)
    {
        var member = await this.GetMemberAsync(person, organization);
        if (member != null)
            return OperationResult.Error("自然人已是该组织的成员。");

        member = new(organization, person)
        {
            Title = title,
            Department = department,
            Remark = remark,
        };
        await this.store.CreateAsync(member);
        return OperationResult.Success;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<OperationResult> CreateAsync(OrganizationMember member)
    {
        if (this.store.OrganizationMembers.Any(p => p.OrganizationId == member.OrganizationId && p.PersonId == member.PersonId))
            return OperationResult.Error("Member already exists.");
        await this.store.CreateAsync(member);
        return OperationResult.Success;
    }

    /// <summary>
    /// Take person leave out the organization.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="organization"></param>
    /// <returns></returns>
    public async Task<OperationResult> LeaveOrganizationAsync(NaturalPerson person, GenericOrganization organization)
    {
        var member = await this.GetMemberAsync(person, organization);
        if (member != null)
        {
            await this.store.DeleteAsync(member);
        }
        return OperationResult.Success;
    }

    /// <summary>
    /// Update organization member info.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<OperationResult> UpdateAsync(OrganizationMember member)
    {
        await this.store.UpdateAsync(member);
        return OperationResult.Success;
    }
}
