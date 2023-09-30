namespace IDSubjects;

/// <summary>
/// Provide CURD for organization.
/// </summary>
public interface IOrganizationStore : IQueryableOrganizationStore
{
    /// <summary>
    /// Find organization by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<GenericOrganization?> FindByIdAsync(string id);

    /// <summary>
    /// Create organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task CreateAsync(GenericOrganization organization);

    /// <summary>
    /// Update organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task UpdateAsync(GenericOrganization organization);

    /// <summary>
    /// Delete organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    Task DeleteAsync(GenericOrganization organization);
}
