namespace IdSubjects.WechatWebLogin;

/// <summary>
/// </summary>
public interface IWechatServiceProvider
{
    /// <summary>
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    Task<string?> GetSecretAsync(string appId);

    /// <summary>
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    Task RegisterAsync(string appId, string secret);

    /// <summary>
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    Task UpdateSecretAsync(string appId, string secret);
}