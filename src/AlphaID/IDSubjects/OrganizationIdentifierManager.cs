using IdSubjects.Validators;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="store"></param>
/// <param name="validators"></param>
public class OrganizationIdentifierManager(IOrganizationIdentifierStore store, IEnumerable<OrganizationIdentifierValidator> validators)
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    public OrganizationIdentifierManager(IOrganizationIdentifierStore store)
        : this(store, [new UsccValidator()])
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AddIdentifierAsync(OrganizationIdentifier identifier)
    {
        List<string> errors = [];
        foreach (var validator in validators)
        {
            errors.AddRange(validator.Validate(identifier).Errors);
        }
        if (errors.Count != 0)
            return IdOperationResult.Failed([.. errors]);
        if (store.Identifiers.Any(i => i.Type == identifier.Type && i.Value == identifier.Value))
            return IdOperationResult.Failed("Identifier has already exists.");
        return await store.CreateAsync(identifier);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public Task<IdOperationResult> RemoveIdentifierAsync(OrganizationIdentifier identifier)
    {
        return store.DeleteAsync(identifier);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public IEnumerable<OrganizationIdentifier> GetIdentifiers(GenericOrganization organization)
    {
        return store.Identifiers.Where(i => i.OrganizationId == organization.Id);
    }
}
