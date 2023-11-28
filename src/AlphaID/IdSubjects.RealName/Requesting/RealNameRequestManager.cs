namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 实名认证请求管理器。
/// </summary>
public class RealNameRequestManager
{
    private readonly IRealNameRequestStore store;
    private readonly RealNameManager realNameManager;
    private readonly IRealNameRequestAuditorProvider? provider;
    private readonly NaturalPersonManager naturalPersonManager;

    /// <summary>
    /// 初始化实名认证请求管理器。
    /// </summary>
    /// <param name="store"></param>
    /// <param name="realNameManager"></param>
    /// <param name="naturalPersonManager"></param>
    /// <param name="provider"></param>
    public RealNameRequestManager(IRealNameRequestStore store, RealNameManager realNameManager, NaturalPersonManager naturalPersonManager, IRealNameRequestAuditorProvider? provider = null)
    {
        this.store = store;
        this.realNameManager = realNameManager;
        this.naturalPersonManager = naturalPersonManager;
        this.provider = provider;
    }

    /// <summary>
    /// Time Provider.
    /// </summary>
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 获取待审核的实名认证请求。
    /// </summary>
    public IEnumerable<RealNameRequest> PendingRequests
    {
        get
        {
            return this.store.Requests.Where(r => !r.Accepted.HasValue);
        }
    }

    /// <summary>
    /// 获取可查询的实名认证请求集合。
    /// </summary>
    public IQueryable<RealNameRequest> Requests => this.store.Requests;

    /// <summary>
    /// 创建实名认证请求。
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
    /// 审核通过一个实名认证请求。
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
    /// 更新实名认证信息。
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
    /// 通过Id查找实名认证请求。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<RealNameRequest?> FindByIdAsync(int id)
    {
        return await this.store.FindByIdAsync(id);
    }

    /// <summary>
    /// 审核拒绝一个实名认证请求。
    /// </summary>
    /// <param name="request"></param>
    /// <param name="auditor"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> RefuseAsync(RealNameRequest request, string? auditor = null)
    {
        request.SetAudit(false, auditor, this.TimeProvider.GetUtcNow());
        return await this.UpdateAsync(request);
    }
}