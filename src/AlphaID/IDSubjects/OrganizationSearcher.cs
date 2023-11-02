namespace IDSubjects;

/// <summary>
/// 组织搜索器。
/// </summary>
public class OrganizationSearcher
{
    private readonly IOrganizationStore _organizationStore;
    private readonly IQueryableOrganizationUsedNameStore organizationUsedNameStore;

    /// <summary>
    /// 初始化组织搜索器。
    /// </summary>
    /// <param name="organizationStore"></param>
    /// <param name="organizationUsedNameStore"></param>
    public OrganizationSearcher(IOrganizationStore organizationStore, IQueryableOrganizationUsedNameStore organizationUsedNameStore)
    {
        this._organizationStore = organizationStore;
        this.organizationUsedNameStore = organizationUsedNameStore;
    }

    /// <summary>
    /// 搜索。
    /// </summary>
    /// <param name="keywords"></param>
    /// <returns></returns>
    public IEnumerable<GenericOrganization> Search(string keywords)
    {
        keywords = keywords.Trim();
        if (string.IsNullOrEmpty(keywords))
            return Enumerable.Empty<GenericOrganization>();

        var result = new HashSet<GenericOrganization>();

        var usedNameResult = this._organizationStore.Organizations.Where(p => p.UsedNames.Any(n => n.Name.Contains(keywords)));
        result.UnionWith(usedNameResult);

        var mainResult = this._organizationStore.Organizations.Where(p => p.Name.Contains(keywords));
        result.UnionWith(mainResult);
        return result;
    }
}
