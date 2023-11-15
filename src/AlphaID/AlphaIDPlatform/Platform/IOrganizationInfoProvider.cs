namespace AlphaIdPlatform.Platform;

/// <summary>
/// Provide GenericOrganization info.
/// </summary>
public interface IOrganizationInfoProvider
{
    /// <summary>
    /// 使用名称、统一社会信用代码等，查找指定企业。
    /// </summary>
    /// <param name="name">要查询的关键字。</param>
    /// <returns>若找到，则返回企业信息，否则返回Null</returns>
    Task<OrganizationInfo?> FindAsync(string name);
}
