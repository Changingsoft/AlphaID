namespace AlphaIdPlatform.JoinOrgRequesting;

/// <summary>
/// 提供用于查询 <see cref="JoinOrganizationRequest"/> 对象的扩展方法。
/// </summary>
/// <remarks>该类包含简化使用 LINQ 过滤和查询 <see cref="JoinOrganizationRequest"/> 集合的方法。</remarks>
public static class QueryableJoinOrganizationRequestExtensions
{
    /// <summary>
    /// 过滤加入组织请求序列，仅包含待审批的请求。
    /// </summary>
    /// <remarks>当 <see cref="JoinOrganizationRequest.IsAccepted"/> 属性为 <see langword="null"/> 时，表示请求处于待审批状态。此方法不会执行查询；它返回一个可进一步组合或执行的可查询对象。</remarks>
    /// <param name="requests">要过滤的 <see cref="JoinOrganizationRequest"/> 对象序列。</param>
    /// <returns>仅包含尚未设置审核状态的请求的 <see cref="IQueryable{T}"/>。</returns>
    public static IQueryable<JoinOrganizationRequest> Pending(this IQueryable<JoinOrganizationRequest> requests)
    {
        return requests.Where(r => !r.IsAccepted.HasValue);
    }
}
