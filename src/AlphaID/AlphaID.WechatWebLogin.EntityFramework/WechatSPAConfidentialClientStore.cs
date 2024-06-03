using IdSubjects.WechatWebLogin;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.WechatWebLogin.EntityFramework;

public class WechatSpaConfidentialClientStore(WechatWebLoginDbContext dbContext) : IWechatAppClientStore
{
    public IQueryable<WechatAppClient> Clients => dbContext.WechatAppClients;

    public async Task CreateAsync(WechatAppClient item)
    {
        dbContext.WechatAppClients.Add(item);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(WechatAppClient item)
    {
        dbContext.WechatAppClients.Remove(item);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<WechatAppClient?> FindAsync(string clientId)
    {
        return await dbContext.WechatAppClients.FindAsync(clientId);
    }

    public async Task UpdateAsync(WechatAppClient item)
    {
        dbContext.Entry(item).State = EntityState.Modified;
        _ = await dbContext.SaveChangesAsync();
    }
}