namespace IdSubjects;

/// <summary>
/// GenericOrganization Manager
/// </summary>
public class OrganizationManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    public OrganizationManager(IOrganizationStore store)
    {
        this.Store = store;
    }

    /// <summary>
    /// 
    /// </summary>
    public IQueryable<GenericOrganization> Organizations => this.Store.Organizations;

    /// <summary>
    /// 获取组织存取器。
    /// </summary>
    protected IOrganizationStore Store { get; }

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 创建一个组织。
    /// </summary>
    /// <param name="org"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> CreateAsync(GenericOrganization org)
    {
        var utcNow = this.TimeProvider.GetUtcNow();
        org.WhenCreated = utcNow;
        org.WhenChanged = utcNow;
        return await this.Store.CreateAsync(org);
    }

    /// <summary>
    /// Find by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [Obsolete("该方法查询的组织不具有跟踪能力，无法用于更改。应使用FindByName")]
    public IEnumerable<GenericOrganization> SearchByName(string name)
    {
        var results = this.Store.Organizations.Where(o => o.Name == name);
        return results;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual IEnumerable<GenericOrganization> FindByName(string name)
    {
        return this.Store.FindByName(name);
    }

    /// <summary>
    /// 使用组织名称尝试查找单个组织。
    /// </summary>
    /// <param name="name">组织的完整名称。</param>
    /// <param name="organization">如果未找到组织，该值为null，如果找到1个组织，该值为该组织，如果找到多个组织，该值为第一个组织。</param>
    /// <returns>如果未找到组织或找到了单个组织，则返回true，否则返回false。</returns>
    public bool TryFindSingleOrDefaultByName(string name, out GenericOrganization? organization)
    {
        var result = this.Store.FindByName(name).ToArray();
        switch (result.Length)
        {
            case 0: organization = null;
                return true;
            case 1:
                organization = result[0];
                return true;
            default:
                organization = result[0];
                return false;
        }
    }

    /// <summary>
    /// Delete GenericOrganization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> DeleteAsync(GenericOrganization organization)
    {
        return await this.Store.DeleteAsync(organization);
    }

    /// <summary>
    /// 通过组织 Id 查找组织。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<GenericOrganization?> FindByIdAsync(string id)
    {
        return await this.Store.FindByIdAsync(id);
    }

    /// <summary>
    /// 通过组织 Id 查找组织。这是同步版本。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GenericOrganization? FindById(string id)
    {
        return this.Store.FindById(id);
    }

    /// <summary>
    /// Update organization information.
    /// </summary>
    /// <param name="org"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> UpdateAsync(GenericOrganization org)
    {
        org.WhenChanged = this.TimeProvider.GetUtcNow();
        return await this.Store.UpdateAsync(org);
    }

    /// <summary>
    /// 更改组织的名称。
    /// </summary>
    /// <param name="org">要更改名称的组织。</param>
    /// <param name="newName">新名称。</param>
    /// <param name="changeDate">更改时间。</param>
    /// <param name="recordUsedName">更改前的名称记录到曾用名。</param>
    /// <param name="applyChangeWhenDuplicated">即便名称重复也要更改。默认为false。</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IdOperationResult> ChangeNameAsync(GenericOrganization org, string newName, DateOnly changeDate, bool recordUsedName, bool applyChangeWhenDuplicated = false)
    {
        var orgId = org.Id;
        newName = newName.Trim().Trim('\r', '\n');
        if (newName == org.Name)
            return IdOperationResult.Failed("名称相同");

        var nameExists = this.Store.Organizations.Any(p => p.Name == newName);
        if (!applyChangeWhenDuplicated && nameExists)
            return IdOperationResult.Failed("存在重复名称");

        if (recordUsedName)
        {
            org.UsedNames.Add(new OrganizationUsedName
            {
                Name = org.Name,
                DeprecateTime = changeDate,
            });
        }
        org.Name = newName;
        await this.UpdateAsync(org);
        return IdOperationResult.Success;
    }
}
