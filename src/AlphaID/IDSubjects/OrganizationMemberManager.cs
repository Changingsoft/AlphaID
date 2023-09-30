namespace IDSubjects;

/// <summary>
/// GenericOrganization Member Manager.
/// </summary>
public class OrganizationMemberManager
{
    private readonly IOrganizationMemberStore store;

    /// <summary>
    /// Init GenericOrganization Member Mamager via GenericOrganization Member store.
    /// </summary>
    /// <param name="store"></param>
    public OrganizationMemberManager(IOrganizationMemberStore store)
    {
        this.store = store;
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
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetMembersAsync(GenericOrganization organization)
    {
        var result = this.store.OrganizationMembers.Where(p => p.OrganizationId == organization.Id);
        return Task.FromResult(result.AsEnumerable());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public Task<IEnumerable<OrganizationMember>> GetMembersOfAsync(NaturalPerson person)
    {
        var result = this.store.OrganizationMembers.Where(p => p.PersonId == person.Id);
        return Task.FromResult(result.AsEnumerable());
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
