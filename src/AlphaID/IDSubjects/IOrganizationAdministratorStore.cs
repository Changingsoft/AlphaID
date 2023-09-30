namespace IDSubjects;

/// <summary>
/// 提供组织的管理者信息持久化能力。
/// </summary>
public interface IOrganizationAdministratorStore
{
    /// <summary>
    /// 获取组织的管理人集合。
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task<IEnumerable<OrganizationAdministrator>> GetAdministrators(GenericOrganization organization);

    /// <summary>
    /// 向组织添加管理人。
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="administrator"></param>
    /// <returns></returns>
    Task AddAdministrator(GenericOrganization organization, OrganizationAdministrator administrator);

    /// <summary>
    /// 向组织移除管理人。
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="administrator"></param>
    /// <returns></returns>
    Task RemoveAdministrator(GenericOrganization organization, OrganizationAdministrator administrator);

    /// <summary>
    /// 更新特定的管理人信息。
    /// </summary>
    /// <param name="administrator"></param>
    /// <returns></returns>
    Task UpdateAdministrator(OrganizationAdministrator administrator);
}
