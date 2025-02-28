using IdSubjects;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// 提供组织成员信息的持久化能力。
/// </summary>
public interface IOrganizationMemberStore
{
    /// <summary>
    /// 获取可查询的组织成员集合。
    /// </summary>
    IQueryable<OrganizationMember> OrganizationMembers { get; }

    /// <summary>
    /// 根据个人Id和组织Id查找组织成员身份。
    /// </summary>
    /// <param name="personId"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    Task<OrganizationMember?> FindAsync(string personId, string organizationId);

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> CreateAsync(OrganizationMember item);

    /// <summary>
    /// Update member.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> UpdateAsync(OrganizationMember item);

    /// <summary>
    /// Delete member.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> DeleteAsync(OrganizationMember item);
}