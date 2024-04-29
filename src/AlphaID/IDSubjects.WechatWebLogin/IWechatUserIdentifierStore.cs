namespace IdSubjects.WechatWebLogin;

/// <summary>
///     提供微信用户标识存储能力。
/// </summary>
public interface IWechatUserIdentifierStore : IQueryableWechatUserIdentifierStore
{
    /// <summary>
    ///     Create wechat user identifier.
    /// </summary>
    /// <param name="item">item to create.</param>
    /// <returns>Asynchronous task</returns>
    Task CreateAsync(WechatUserIdentifier item);

    /// <summary>
    ///     Update wechat user identifier.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task UpdateAsync(WechatUserIdentifier item);

    /// <summary>
    ///     Delete wechat user identifier from persistence.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task DeleteAsync(WechatUserIdentifier item);
}