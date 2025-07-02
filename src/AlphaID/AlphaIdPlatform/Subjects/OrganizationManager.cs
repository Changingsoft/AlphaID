namespace AlphaIdPlatform.Subjects;

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
    /// 通过名称查找组织。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ValueTask<Organization?> FindByNameAsync(string name)
    {
        var org = store.Organizations.FirstOrDefault(o => o.Name == name);
        return ValueTask.FromResult(org);
    }

    /// <summary>
    /// Delete Organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public Task<OrganizationOperationResult> DeleteAsync(Organization organization)
    {
        return store.DeleteAsync(organization);
    }

    /// <summary>
    /// 通过组织 Id 查找组织。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Organization?> FindByIdAsync(string id)
    {
        return store.FindByIdAsync(id);
    }

    /// <summary>
    /// Update organization information.
    /// </summary>
    /// <param name="org"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> UpdateAsync(Organization org)
    {
        if (Organizations.Any(p => p.Name == org.Name && p.Id != org.Id))
            return OrganizationOperationResult.Failed("名称重复");
        org.WhenChanged = TimeProvider.GetUtcNow();
        return await store.UpdateAsync(org);
    }

    /// <summary>
    /// 更改组织的名称。
    /// </summary>
    /// <param name="org">要更改名称的组织。</param>
    /// <param name="newName">新名称。</param>
    /// <param name="changeDate">更改时间。</param>
    /// <param name="recordUsedName">是否将原名称记入曾用名。默认为true。</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<OrganizationOperationResult> RenameAsync(Organization org, string newName, DateOnly? changeDate = null, bool recordUsedName = true)
    {
        if (newName == org.Name)
            return OrganizationOperationResult.Failed("新名称与原名称相同。");

        if (recordUsedName)
        {
            org.UsedNames.Add(new OrganizationUsedName
            {
                Name = org.Name,
                //使用本地时间以避免早上8点前日期被减一天。
                DeprecateTime = changeDate ?? DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime),
            });
        }

        org.Name = newName;
        return await UpdateAsync(org);
    }
}