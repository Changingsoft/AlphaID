namespace IDSubjects;

/// <summary>
/// 提供组织的曾用名查询能力。
/// </summary>
public interface IQueryableOrganizationUsedNameStore
{
    /// <summary>
    /// 获取可查询的组织曾用名集合。
    /// </summary>
    IQueryable<OrganizationUsedName> UsedNames { get; }
}