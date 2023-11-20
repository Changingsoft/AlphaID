namespace IdSubjects;

/// <summary>
/// 组织搜索器。
/// </summary>
public class OrganizationSearcher
{
    private readonly IOrganizationStore organizationStore;

    /// <summary>
    /// 初始化组织搜索器。
    /// </summary>
    /// <param name="organizationStore"></param>
    public OrganizationSearcher(IOrganizationStore organizationStore)
    {
        this.organizationStore = organizationStore;
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

        var mainResult = this.organizationStore.Organizations.Where(p => p.Name.Contains(keywords) || p.UsedNames.Any(n => n.Name.Contains(keywords)));
        
        result.UnionWith(mainResult);
        return result;
    }
}
