using Microsoft.AspNetCore.Identity;

namespace IDSubjects;

/// <summary>
/// 提供组织成员信息的持久化能力。
/// </summary>
public interface IOrganizationMemberStore
{
    /// <summary>
    /// Gets queryable of GenericOrganization member.
    /// </summary>
    IQueryable<OrganizationMember> OrganizationMembers { get; }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task CreateAsync(OrganizationMember item);

    /// <summary>
    /// Update member.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<IdentityResult> UpdateAsync(OrganizationMember item);

    /// <summary>
    /// Delete member.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<IdentityResult> DeleteAsync(OrganizationMember item);
}