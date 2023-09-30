namespace AlphaIDWebAPI.Models;

/// <summary>
/// 组织机构概要
/// </summary>
public class OrganizationSearchResult
{
    /// <summary>
    /// Initialize by organizations and more flag.
    /// </summary>
    /// <param name="organizations"></param>
    /// <param name="more"></param>
    public OrganizationSearchResult(IEnumerable<OrganizationModel> organizations, bool more = false)
    {
        this.Organizations = organizations;
        this.More = more;
    }

    /// <summary>
    /// 此查找结果包含的组织信息。
    /// </summary>
    public IEnumerable<OrganizationModel> Organizations { get; set; }

    /// <summary>
    /// 指示出组织信息集合外，是否还有更多结果未返回。这意味着关键字所匹配结果集较大，需要重新选择关键字以便缩小匹配范围。
    /// </summary>
    public bool More { get; set; }
}