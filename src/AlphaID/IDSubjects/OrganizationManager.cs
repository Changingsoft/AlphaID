using System.Transactions;

namespace IDSubjects;

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

    /// <summary>
    /// 获取一个值，指示是否支持组织的管理人管理功能。
    /// </summary>
    public bool SupportOrganizationAdministratorManagement => this.OrganizationStore is IOrganizationAdministratorStore;

    /// <summary>
    /// 创建一个组织。
    /// </summary>
    /// <param name="org"></param>
    /// <param name="creator"></param>
    /// <returns></returns>
    public async Task<OperationResult> CreateAsync(GenericOrganization org, PersonInfo? creator = null)
    {
        await this.OrganizationStore.CreateAsync(org);
        if (this.OrganizationStore is IOrganizationAdministratorStore administratorStore)
        {
            if (creator != null)
            {
                OrganizationAdministrator administrator = new()
                {
                    OrganizationId = org.Id,
                    PersonId = creator.Id,
                    Name = creator.Name,
                    IsOrganizationCreator = true
                };
                await administratorStore.AddAdministrator(org, administrator);
            }
        }
        return OperationResult.Success;
    }

    /// <summary>
    /// 获取某组织的管理者。
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Task<IEnumerable<OrganizationAdministrator>> GetAdministratorsAsync(GenericOrganization organization)
    {
        return this.OrganizationStore is not IOrganizationAdministratorStore administratorStore
            ? throw new NotSupportedException("不支持该功能")
            : administratorStore.GetAdministrators(organization);
    }

    /// <summary>
    /// 向组织添加一个管理员。
    /// 如果指示了asCreator，那么除了添加为管理员，也会将其设置为组织的创建者。
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="person"></param>
    /// <param name="asCreator"></param>
    /// <returns></returns>
    public async Task<OperationResult> AddAdministratorAsync(GenericOrganization organization, PersonInfo person, bool asCreator = false)
    {
        if (this.OrganizationStore is not IOrganizationAdministratorStore administratorStore)
            throw new NotSupportedException("不支持该功能");

        List<string> errors = new();
        if ((await administratorStore.GetAdministrators(organization)).Any(p => p.PersonId == person.Id))
            errors.Add("当前用户已经是组织的管理人");

        if (errors.Any())
            return new OperationResult(errors);

        OrganizationAdministrator administrator = new()
        {
            OrganizationId = organization.Id,
            PersonId = person.Id,
            Name = person.Name,
            IsOrganizationCreator = asCreator,
        };

        using TransactionScope trans = new(TransactionScopeAsyncFlowOption.Enabled);
        if (asCreator)
        {
            var currentCreators = (await administratorStore.GetAdministrators(organization)).Where(p => p.IsOrganizationCreator);
            foreach (var creator in currentCreators)
            {
                creator.IsOrganizationCreator = false;
                await administratorStore.UpdateAdministrator(creator);
            }
        }

        await administratorStore.AddAdministrator(organization, administrator);
        trans.Complete();
        return OperationResult.Success;
    }

    /// <summary>
    /// 将指定的用户从组织管理人中移除
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="administrator"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<OperationResult> RemoveAdministratorAsync(GenericOrganization organization, OrganizationAdministrator administrator)
    {
        if (this.OrganizationStore is not IOrganizationAdministratorStore administratorStore)
            throw new NotSupportedException("不支持该功能");

        List<string> errors = new();
        if (administrator.IsOrganizationCreator)
        {
            errors.Add("此人是组织的创建者，必须现将组织的创建者转移给其他管理人，才能将该管理人移除");
        }
        if (errors.Any())
            return new OperationResult(errors);

        await administratorStore.RemoveAdministrator(organization, administrator);
        return OperationResult.Success;
    }

    /// <summary>
    /// 将某个组织管理人设置为创建者。
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="administrator"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<OperationResult> SetCreatorAsync(GenericOrganization organization, OrganizationAdministrator administrator)
    {
        if (this.OrganizationStore is not IOrganizationAdministratorStore administratorStore)
            throw new NotSupportedException("不支持该功能");

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var administrators = await administratorStore.GetAdministrators(organization);
        foreach (var oldCreator in administrators.Where(p => p.IsOrganizationCreator))
        {
            oldCreator.IsOrganizationCreator = false;
            await administratorStore.UpdateAdministrator(oldCreator);
        }
        administrator.IsOrganizationCreator = true;
        await administratorStore.UpdateAdministrator(administrator);
        trans.Complete();
        return OperationResult.Success;
    }

    /// <summary>
    /// Find by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IEnumerable<GenericOrganization>> SearchByNameAsync(string name)
    {
        var results = this.OrganizationStore.Organizations.Where(o => o.Name == name);
        return Task.FromResult(results.AsEnumerable());
    }

    /// <summary>
    /// Delete GenericOrganization.
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public async Task<OperationResult> DeleteAsync(GenericOrganization organization)
    {
        await this.OrganizationStore.DeleteAsync(organization);
        return OperationResult.Success;
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
    public async Task UpdateAsync(GenericOrganization org)
    {
        org.WhenChanged = DateTime.UtcNow;
        await this.OrganizationStore.UpdateAsync(org);
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
    public async Task<OperationResult> ChangeNameAsync(GenericOrganization org, string newName, DateTime changeDate, bool recordUsedName, bool applyChangeWhenDuplicated = false)
    {
        var orgId = org.Id;
        newName = newName.Trim().Trim('\r', '\n');
        if (string.IsNullOrEmpty(newName))
            return OperationResult.Error("新名称无效");
        if (newName == org.Name)
            return OperationResult.Error("名称相同");

        var checkDuplicate = this.OrganizationStore.Organizations.Any(p => p.Name == newName);
        if (applyChangeWhenDuplicated && checkDuplicate)
            return OperationResult.Error("存在重复名称");

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
        return OperationResult.Success;
    }

    /// <summary>
    /// 为组织设置其地理坐标位置，采用WGS-84。
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="lon"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public virtual async Task<OperationResult> SetLocation(GenericOrganization organization, double lon, double lat)
    {
        var factory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        organization.Location = factory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(lon, lat));
        await this.OrganizationStore.UpdateAsync(organization);
        return OperationResult.Success;
    }
}
