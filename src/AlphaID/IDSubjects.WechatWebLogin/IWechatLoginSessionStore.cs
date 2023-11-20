namespace IdSubjects.WechatWebLogin;

/// <summary>
/// 
/// </summary>
public interface IWechatLoginSessionStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    Task<WechatLoginSession?> FindAsync(string sessionId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    Task CreateAsync(WechatLoginSession session);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task CleanExpiredSessionsAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    Task UpdateAsync(WechatLoginSession session);
}