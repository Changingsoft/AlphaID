namespace IdSubjects;

/// <summary>
/// 
/// </summary>
public interface IOrganizationIdentifierStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<OrganizationIdentifier> Identifiers { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(OrganizationIdentifier  identifier);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(OrganizationIdentifier identifier);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(OrganizationIdentifier identifier);
}
