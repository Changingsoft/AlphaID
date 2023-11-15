namespace AlphaIdWebAPI.Models;



/// <summary>
/// 组织机构概要
/// </summary>
/// <param name="Organizations"> 此查找结果包含的组织信息。 </param>
/// <param name="More"> 指示出组织信息集合外，是否还有更多结果未返回。这意味着关键字所匹配结果集较大，需要重新选择关键字以便缩小匹配范围。 </param>
public record OrganizationSearchResult(IEnumerable<OrganizationModel> Organizations, bool More = false);
