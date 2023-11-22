namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 
/// </summary>
public class RealNameRequestManager
{
    private readonly IRealNameRequestStore store;
    private readonly RealNameManager realNameManager;
    private readonly IRealNameRequestAuditorProvider? provider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    /// <param name="realNameManager"></param>
    /// <param name="provider"></param>
    public RealNameRequestManager(IRealNameRequestStore store, RealNameManager realNameManager, IRealNameRequestAuditorProvider? provider = null)
    {
        this.store = store;
        this.realNameManager = realNameManager;
        this.provider = provider;
    }

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> CreateAsync(NaturalPerson person, RealNameRequest request)
    {
        request.PersonId = person.Id;
        var result = await this.store.CreateAsync(request);
        if (!result.Succeeded)
            return result;

        if (this.provider == null)
        {
            return result;
        }

        var auditors = this.provider.GetAuditors();
        var accept = true;
        foreach (var auditor in auditors)
        {
            if (!await auditor.ValidateAsync(request))
                accept = false;
        }

        if (!accept)
            return result;

        return await this.AcceptAsync(person, request);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="naturalPerson"></param>
    /// <param name="request"></param>
    /// <param name="auditor"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AcceptAsync(NaturalPerson naturalPerson, RealNameRequest request, string? auditor = null)
    {
        request.Accept(auditor, this.TimeProvider.GetUtcNow());
        var result = await this.UpdateAsync(request);
        if (!result.Succeeded)
            return result;

        var authentication = request.CreateAuthentication();
        return await this.realNameManager.AuthenticateAsync(naturalPerson, authentication);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<IdOperationResult> UpdateAsync(RealNameRequest request)
    {
        return await this.store.UpdateAsync(request);
        throw new NotImplementedException();
    }
}