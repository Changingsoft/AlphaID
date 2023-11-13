using IDSubjects.WechatWebLogin;
using Microsoft.EntityFrameworkCore;

namespace AlphaID.WechatWebLogin.EntityFramework;

public class WechatSpaConfidentialClientStore : IWechatAppClientStore
{
    private readonly WechatWebLoginDbContext dbContext;

    public WechatSpaConfidentialClientStore(WechatWebLoginDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<WechatAppClient> Clients => this.dbContext.WechatAppClients;

    public async Task CreateAsync(WechatAppClient item)
    {
        this.dbContext.WechatAppClients.Add(item);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(WechatAppClient item)
    {
        this.dbContext.WechatAppClients.Remove(item);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<WechatAppClient?> FindAsync(string clientId)
    {
        return await this.dbContext.WechatAppClients.FindAsync(clientId);
    }

    public async Task UpdateAsync(WechatAppClient item)
    {
        this.dbContext.Entry(item).State = EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
    }
}
