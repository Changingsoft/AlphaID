using IdSubjects;

namespace AlphaIdPlatform.Subjects;

/// <summary>
///     Provide CURD for organization.
/// </summary>
public interface IOrganizationStore
{
    /// <summary>
    ///     Gets queryable of organization.
    /// </summary>
    IQueryable<Organization> Organizations { get; }

    /// <summary>
    ///     Find organization by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Organization?> FindByIdAsync(string id);

    /// <summary>
    ///     通过组织 Id 查找组织。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Organization? FindById(string id);

    /// <summary>
    ///     通过组织名称查找组织。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEnumerable<Organization> FindByName(string name);

    /// <summary>
    ///     Create organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> CreateAsync(Organization organization);

    /// <summary>
    ///     Update organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> UpdateAsync(Organization organization);

    /// <summary>
    ///     Delete organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> DeleteAsync(Organization organization);
}