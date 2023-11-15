namespace IdSubjects;

/// <summary>
/// GenericOrganization Manager
/// </summary>
public class OrganizationManager
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organizationStore"></param>
    public OrganizationManager(IOrganizationStore organizationStore)
    {
        this.OrganizationStore = organizationStore;
    }

    /// <summary>
    /// 
    /// </summary>
    public IQueryable<GenericOrganization> Organizations => this.OrganizationStore.Organizations;

    /// <summary>
    /// Gets or sets GenericOrganization store.
    /// </summary>
    protected IOrganizationStore OrganizationStore { get; set; }

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
        return await this.OrganizationStore.CreateAsync(org);
    }

    /// <summary>
    /// Find by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEnumerable<GenericOrganization> SearchByName(string name)
    {
        var results = this.OrganizationStore.Organizations.Where(o => o.Name == name);
        return results;
    }

    /// <summary>
    /// Delete GenericOrganization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> DeleteAsync(GenericOrganization organization)
    {
        return await this.OrganizationStore.DeleteAsync(organization);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<GenericOrganization?> FindByIdAsync(string id)
    {
        return await this.OrganizationStore.FindByIdAsync(id);
    }

    /// <summary>
    /// Update organization information.
    /// </summary>
    /// <param name="org"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> UpdateAsync(GenericOrganization org)
    {
        org.WhenChanged = this.TimeProvider.GetUtcNow();
        return await this.OrganizationStore.UpdateAsync(org);
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

        var nameExists = this.OrganizationStore.Organizations.Any(p => p.Name == newName);
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

    /// <summary>
    /// 为组织设置其地理坐标位置，采用WGS-84。
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="lon"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public virtual async Task<IdOperationResult> SetLocation(GenericOrganization organization, double lon, double lat)
    {
        var factory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        organization.Location = factory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(lon, lat));
        await this.OrganizationStore.UpdateAsync(organization);
        return IdOperationResult.Success;
    }
}
