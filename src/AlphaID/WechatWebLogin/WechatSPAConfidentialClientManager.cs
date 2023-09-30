namespace WechatWebLogin;

/// <summary>
/// 
/// </summary>
public class WechatSPAConfidentialClientManager
{
    private readonly IWechatAppClientStore store;

    /// <summary>
    /// Initialize manager via store.
    /// </summary>
    /// <param name="store"></param>
    public WechatSPAConfidentialClientManager(IWechatAppClientStore store)
    {
        this.store = store;
    }

    /// <summary>
    /// Register a new wechat SAP confidential client.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    public async Task<WechatAppClient> Register(string clientId, string secret)
    {
        var exists = await this.store.FindAsync(clientId);
        if (exists != null)
            throw new InvalidOperationException("已存在客户端");

        var client = new WechatAppClient(clientId, secret);
        await this.store.CreateAsync(client);
        return client;
    }

    /// <summary>
    /// Update secret of registered client
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    public async Task<WechatAppClient> UpdateSecretAsync(string clientId, string secret)
    {
        var exists = await this.store.FindAsync(clientId) ?? throw new ArgumentException("找不到客户端注册信息。");
        exists.Secret = secret;
        await this.store.UpdateAsync(exists);
        return exists;
    }

    /// <summary>
    /// Remove client registration from store.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public async Task RemoveAsync(string clientId)
    {
        var client = await this.store.FindAsync(clientId);
        if (client == null)
            return;

        await this.store.DeleteAsync(client);

    }
}
