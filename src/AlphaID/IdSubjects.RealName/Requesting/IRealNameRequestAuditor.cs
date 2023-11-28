namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 表示一个实名认证请求的审核器。
/// </summary>
public interface IRealNameRequestAuditor
{
    /// <summary>
    /// 审核一个实名认证请求。
    /// </summary>
    /// <param name="request">要审核的请求。</param>
    /// <returns>如果审核不通过，请返回 false。</returns>
    Task<bool> ValidateAsync(RealNameRequest request);
}