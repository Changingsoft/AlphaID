namespace WechatWebLogin;

/// <summary>
/// 
/// </summary>
public interface IQueryableWechatUserIdentifierStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<WechatUserIdentifier> WechatUserIdentifiers { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wxAppId"></param>
    /// <param name="openId"></param>
    /// <returns></returns>
    Task<WechatUserIdentifier?> FindAsync(string wxAppId, string openId);
}
