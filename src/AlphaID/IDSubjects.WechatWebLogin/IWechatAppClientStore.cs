namespace IdSubjects.WechatWebLogin;

/// <summary>
/// </summary>
public interface IWechatAppClientStore
{
    /// <summary>
    /// </summary>
    IQueryable<WechatAppClient> Clients { get; }

    /// <summary>
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    Task<WechatAppClient?> FindAsync(string clientId);

    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task CreateAsync(WechatAppClient item);

    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task UpdateAsync(WechatAppClient item);

    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task DeleteAsync(WechatAppClient item);
}