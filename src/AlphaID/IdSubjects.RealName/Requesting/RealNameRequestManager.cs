namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 
/// </summary>
public class RealNameRequestManager
{
    private readonly IRealNameRequestStore store;
    private readonly RealNameManager realNameManager;
    private readonly IRealNameRequestAuditorProvider? provider;
    NaturalPersonManager naturalPersonManager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    /// <param name="realNameManager"></param>
    /// <param name="provider"></param>
    public RealNameRequestManager(IRealNameRequestStore store, RealNameManager realNameManager, NaturalPersonManager naturalPersonManager, IRealNameRequestAuditorProvider? provider = null)
    {
        this.store = store;
        this.realNameManager = realNameManager;
        this.naturalPersonManager = naturalPersonManager;
        this.provider = provider;
    }

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<RealNameRequest> PendingRequests
    {
        get
        {
            return this.store.Requests.Where(r => !r.Accepted.HasValue);
        }
    }

    public IQueryable<RealNameRequest> Requests => this.store.Requests;
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

        return await this.AcceptAsync(request);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="auditor"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AcceptAsync(RealNameRequest request, string? auditor = null)
    {
        var person = await this.naturalPersonManager.FindByIdAsync(request.PersonId);
        if (person == null)
            return IdOperationResult.Failed("Natural person not found.");
        request.SetAudit(true, auditor, this.TimeProvider.GetUtcNow());
        var result = await this.UpdateAsync(request);
        if (!result.Succeeded)
            return result;

        var authentication = request.CreateAuthentication();
        return await this.realNameManager.AuthenticateAsync(person, authentication);
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
    }

    /// <summary>
    /// 获取与自然人关联的实名认真请求。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public IEnumerable<RealNameRequest> GetRequests(NaturalPerson person)
    {
        return this.store.Requests.Where(x => x.PersonId == person.Id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="anchor"></param>
    /// <returns></returns>
    public async Task<RealNameRequest?> FindByIdAsync(int anchor)
    {
        return await this.store.FindByIdAsync(anchor);
    }

    public async Task<IdOperationResult> RefuseAsync(RealNameRequest request, string? auditor = null)
    {
        request.SetAudit(false, auditor, this.TimeProvider.GetUtcNow());
        return await this.UpdateAsync(request);
    }
}