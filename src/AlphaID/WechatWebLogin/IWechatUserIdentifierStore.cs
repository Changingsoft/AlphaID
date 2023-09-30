namespace WechatWebLogin;

/// <summary>
/// 提供微信用户标识存储能力。
/// </summary>
public interface IWechatUserIdentifierStore : IQueryableWechatUserIdentifierStore
{
    /// <summary>
    /// Create wechat user identifer.
    /// </summary>
    /// <param name="item">item to create.</param>
    /// <returns>Asynchronous task</returns>
    Task CreateAsync(WechatUserIdentifier item);

    /// <summary>
    /// Update wechat user identifer.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task UpdateAsync(WechatUserIdentifier item);

    /// <summary>
    /// Delete wechat user identifer from persistance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task DeleteAsync(WechatUserIdentifier item);
}
