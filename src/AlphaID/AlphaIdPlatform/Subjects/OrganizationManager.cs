using IdSubjects;

namespace AlphaIdPlatform.Subjects;

/// <summary>
///     Organization Manager
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="store"></param>
public class OrganizationManager(IOrganizationStore store)
{
    /// <summary>
    /// </summary>
    public IQueryable<Organization> Organizations => Store.Organizations;

    /// <summary>
    ///     获取组织存取器。
    /// </summary>
    protected IOrganizationStore Store { get; } = store;

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    ///     创建一个组织。
    /// </summary>
    /// <param name="org"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> CreateAsync(Organization org)
    {
        if (Store.Organizations.Any(p => p.Name == org.Name))
            return OrganizationOperationResult.Failed("名称重复");
        org.WhenCreated = TimeProvider.GetUtcNow();
        org.WhenChanged = org.WhenCreated;
        return await Store.CreateAsync(org);
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual IEnumerable<Organization> FindByName(string name)
    {
        return Store.Organizations.Where(p => p.Name == name || p.UsedNames.Any(q => q.Name == name));
    }

    /// <summary>
    /// 按现用名称查找组织。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual Organization? FindByCurrentName(string name)
    {
        return Store.Organizations.FirstOrDefault(p => p.Name == name);
    }

    /// <summary>
    ///     Delete Organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public Task<OrganizationOperationResult> DeleteAsync(Organization organization)
    {
        return Store.DeleteAsync(organization);
    }

    /// <summary>
    ///     通过组织 Id 查找组织。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Organization?> FindByIdAsync(string id)
    {
        return Store.FindByIdAsync(id);
    }

    /// <summary>
    ///     Update organization information.
    /// </summary>
    /// <param name="org"></param>
    /// <returns></returns>
    public async Task<OrganizationOperationResult> UpdateAsync(Organization org)
    {
        if (Store.Organizations.Any(p => p.Name == org.Name && p.Id != org.Id))
            return OrganizationOperationResult.Failed("名称重复");
        org.WhenChanged = TimeProvider.GetUtcNow();
        return await Store.UpdateAsync(org);
    }

    /// <summary>
    ///     更改组织的名称。
    /// </summary>
    /// <param name="org">要更改名称的组织。</param>
    /// <param name="newName">新名称。</param>
    /// <param name="changeDate">更改时间。</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<OrganizationOperationResult> RenameAsync(Organization org, string newName, DateOnly? changeDate = null)
    {
        newName = newName.Trim().Trim('\r', '\n');
        if (newName == org.Name)
            return OrganizationOperationResult.Failed("名称相同。");

        org.UsedNames.Add(new OrganizationUsedName
        {
            Name = org.Name,
            //使用本地时间以避免早上8点前日期被减一天。
            DeprecateTime = changeDate ?? DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime),
        });
        org.Name = newName;
        return await UpdateAsync(org);
    }
}