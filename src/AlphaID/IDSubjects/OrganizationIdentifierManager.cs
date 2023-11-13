namespace IDSubjects;

/// <summary>
/// 
/// </summary>
public class OrganizationIdentifierManager
{
    private readonly IOrganizationIdentifierStore store;
    private readonly IEnumerable<OrganizationIdentifierValidator> validators;

    public OrganizationIdentifierManager(IOrganizationIdentifierStore store)
        : this(store, new List<OrganizationIdentifierValidator>() { new UsccValidator() })
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    public OrganizationIdentifierManager(IOrganizationIdentifierStore store, IEnumerable<OrganizationIdentifierValidator> validators)
    {
        this.store = store;
        this.validators = validators;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AddIdentifierAsync(OrganizationIdentifier identifier)
    {
        List<string> errors = new();
        foreach (var validator in this.validators)
        {
            errors.AddRange(validator.Validate(identifier).Errors);
        }
        if (errors.Any())
            return IdOperationResult.Failed(errors.ToArray());
        if (this.store.Identifiers.Any(i => i.Type == identifier.Type && i.Value == identifier.Value))
            return IdOperationResult.Failed("Identifier has already exists.");
        return await this.store.CreateAsync(identifier);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> RemoveIdentifierAsync(OrganizationIdentifier identifier)
    {
        return await this.store.DeleteAsync(identifier);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public IEnumerable<OrganizationIdentifier> GetIdentifiers(GenericOrganization organization)
    {
        return this.store.Identifiers.Where(i => i.OrganizationId == organization.Id);
    }
}
