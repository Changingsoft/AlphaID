namespace AlphaIdPlatform.JoinOrgRequesting;


/// <summary>
/// 加入组织请求存储接口。
/// </summary>
public interface IJoinOrganizationRequestStore
{
    /// <summary>
    /// 获取所有加入组织请求的查询集合。
    /// </summary>
    IQueryable<JoinOrganizationRequest> Requests { get; }

    /// <summary>
    /// 创建新的加入组织请求。
    /// </summary>
    /// <param name="item">要创建的请求实体。</param>
    Task CreateAsync(JoinOrganizationRequest item);

    /// <summary>
    /// 更新已有的加入组织请求。
    /// </summary>
    /// <param name="item">要更新的请求实体。</param>
    Task UpdateAsync(JoinOrganizationRequest item);

    /// <summary>
    /// 删除指定的加入组织请求。
    /// </summary>
    /// <param name="item">要删除的请求实体。</param>
    Task DeleteAsync(JoinOrganizationRequest item);
}
