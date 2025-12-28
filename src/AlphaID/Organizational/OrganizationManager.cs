using System.Transactions;

namespace Organizational;

/// <summary>
/// 组织管理器。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="store"></param>
public class OrganizationManager(IOrganizationStore store)
{
    /// <summary>
    /// </summary>
    public IQueryable<Organization> Organizations => store.Organizations;


    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 创建一个组织。
    /// </summary>
    /// <param name="org"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> CreateAsync(Organization org)
    {
        if (Organizations.Any(p => p.Name == org.Name))
            return OrganizationOperationResult.Failed("名称重复");
        org.WhenCreated = TimeProvider.GetUtcNow();
        org.WhenChanged = org.WhenCreated;
        return await store.CreateAsync(org);
    }

    /// <summary>
    /// 通过名称查找组织。该方法仅考虑组织的当前名称，不考虑曾用名。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ValueTask<Organization?> FindByNameAsync(string name)
    {
        var org = store.Organizations.FirstOrDefault(o => o.Name == name);
        return ValueTask.FromResult(org);
    }

    /// <summary>
    /// 更改组织的名称。
    /// </summary>
    /// <param name="org">要更改名称的组织。</param>
    /// <param name="newName">新名称。</param>
    /// <param name="changeDate">更改时间。</param>
    /// <param name="recordUsedName">是否将原名称记入曾用名。默认为true。</param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> ChangeName(string orgId, string newName, DateOnly? changeDate = null, bool recordUsedName = true)
    {
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var org = await store.FindByIdAsync(orgId);
        if (org == null)
            return OrganizationOperationResult.Failed("未找到指定的组织。");

        if (newName == org.Name)
            return OrganizationOperationResult.Failed("新名称与原名称相同。");

        if(store.Organizations.Any(o => o.Name == newName))
            return OrganizationOperationResult.Failed("名称已被使用。");

        //使用本地时间以避免早上8点前日期被减一天。
        var deprecateTime = changeDate ?? DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime);

        org.SetName(newName, recordUsedName, deprecateTime);
        
        var result = await store.UpdateAsync(org);
        
        trans.Complete();
        return result;
    }
}