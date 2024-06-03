namespace IdSubjects;

/// <summary>
///     组织标识符验证器。
/// </summary>
public abstract class OrganizationIdentifierValidator
{
    /// <summary>
    ///     验证一个组织的标识符是否符合要求。
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public abstract IdOperationResult Validate(OrganizationIdentifier identifier);
}