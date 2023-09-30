using IDSubjects;

namespace AlphaIDWebAPI.Models;

/// <summary>
/// 组织的成员。
/// </summary>
/// <param name="Department">部门</param>
/// <param name="Title">职务</param>
/// <param name="PersonId">自然人标识符</param>
/// <param name="PersonName">自然人名称</param>
/// <param name="OrganizationId">组织标识符</param>
/// <param name="OrganizationName">组织名称</param>
/// <param name="Remark">备注</param>
public record OrganizationMemberModel(string PersonId,
                                      string PersonName,
                                      string OrganizationId,
                                      string OrganizationName,
                                      string? Title,
                                      string? Department,
                                      string? Remark)
{
    /// <summary>
    /// Init.
    /// </summary>
    /// <param name="member"></param>
    public OrganizationMemberModel(OrganizationMember member)
        : this(member.PersonId,
               member.Person.Name,
               member.OrganizationId,
               member.Organization.Name,
               member.Title,
               member.Department,
               member.Remark)
    {
    }
}
