namespace IdSubjects;

/// <summary>
///     Provide CURD for organization.
/// </summary>
public interface IOrganizationStore
{
    /// <summary>
    ///     Gets queryable of organization.
    /// </summary>
    IQueryable<GenericOrganization> Organizations { get; }

    /// <summary>
    ///     Find organization by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<GenericOrganization?> FindByIdAsync(string id);

    /// <summary>
    ///     通过组织 Id 查找组织。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    GenericOrganization? FindById(string id);

    /// <summary>
    ///     通过组织名称查找组织。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEnumerable<GenericOrganization> FindByName(string name);

    /// <summary>
    ///     Create organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(GenericOrganization organization);

    /// <summary>
    ///     Update organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(GenericOrganization organization);

    /// <summary>
    ///     Delete organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(GenericOrganization organization);
}