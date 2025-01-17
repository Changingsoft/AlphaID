namespace IdSubjects;

/// <summary>
///     组织的标识符存取接口。
/// </summary>
public interface IOrganizationIdentifierStore
{
    /// <summary>
    ///     组织的标识符。
    /// </summary>
    IQueryable<OrganizationIdentifier> Identifiers { get; }

    /// <summary>
    ///     创建组织的标识符。
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(OrganizationIdentifier identifier);

    /// <summary>
    ///     更新组织的标识符。
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(OrganizationIdentifier identifier);

    /// <summary>
    ///     删除组织的标识符。
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(OrganizationIdentifier identifier);
}