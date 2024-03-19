namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 实名认证请求管理器。
/// </summary>
/// <remarks>
/// 初始化实名认证请求管理器。
/// </remarks>
/// <param name="store"></param>
/// <param name="realNameManager"></param>
/// <param name="naturalPersonManager"></param>
/// <param name="provider"></param>
public class RealNameRequestManager(IRealNameRequestStore store, RealNameManager realNameManager, NaturalPersonManager naturalPersonManager, IRealNameRequestAuditorProvider? provider = null)
{

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
            return store.Requests.Where(r => !r.Accepted.HasValue);
        }
    }

    /// <summary>
    /// 获取可查询的实名认证请求集合。
    /// </summary>
    public IQueryable<RealNameRequest> Requests => store.Requests;

    /// <summary>
    /// 创建实名认证请求。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> CreateAsync(NaturalPerson person, RealNameRequest request)
    {
        request.PersonId = person.Id;
        request.WhenCommitted = this.TimeProvider.GetUtcNow();
        var result = await store.CreateAsync(request);
        if (!result.Succeeded)
            return result;

        if (provider == null)
        {
            return result;
        }

        var auditors = provider.GetAuditors();
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
        var person = await naturalPersonManager.FindByIdAsync(request.PersonId);
        if (person == null)
            return IdOperationResult.Failed("Natural person not found.");
        request.SetAudit(true, auditor, this.TimeProvider.GetUtcNow());
        var result = await this.UpdateAsync(request);
        if (!result.Succeeded)
            return result;

        var authentication = request.CreateAuthentication();
        return await realNameManager.AuthenticateAsync(person, authentication);
    }

    /// <summary>
    /// 更新实名认证信息。
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private Task<IdOperationResult> UpdateAsync(RealNameRequest request)
    {
        return store.UpdateAsync(request);
    }

    /// <summary>
    /// 获取与自然人关联的实名认真请求。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public IEnumerable<RealNameRequest> GetRequests(NaturalPerson person)
    {
        return store.Requests.Where(x => x.PersonId == person.Id);
    }

    /// <summary>
    /// 通过Id查找实名认证请求。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<RealNameRequest?> FindByIdAsync(int id)
    {
        return store.FindByIdAsync(id);
    }

    /// <summary>
    /// 审核拒绝一个实名认证请求。
    /// </summary>
    /// <param name="request"></param>
    /// <param name="auditor"></param>
    /// <returns></returns>
    public Task<IdOperationResult> RefuseAsync(RealNameRequest request, string? auditor = null)
    {
        request.SetAudit(false, auditor, this.TimeProvider.GetUtcNow());
        return this.UpdateAsync(request);
    }
}