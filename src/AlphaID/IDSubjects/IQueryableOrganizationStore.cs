namespace IDSubjects;

/// <summary>
/// Provide Queryable for GenericOrganization.
/// </summary>
public interface IQueryableOrganizationStore
{
    /// <summary>
    /// Gets queryable of organization.
    /// </summary>
    IQueryable<GenericOrganization> Organizations { get; }

    /// <summary>
    /// Find organization by identity.
    /// </summary>
    /// <param name="identityType"></param>
    /// <param name="identityValue"></param>
    /// <returns></returns>
    Task<GenericOrganization?> FindByIdentityAsync(string identityType, string identityValue);
}
