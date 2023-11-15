namespace IdSubjects;

/// <summary>
/// 
/// </summary>
public abstract class OrganizationIdentifierValidator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public abstract IdOperationResult Validate(OrganizationIdentifier identifier);
}
