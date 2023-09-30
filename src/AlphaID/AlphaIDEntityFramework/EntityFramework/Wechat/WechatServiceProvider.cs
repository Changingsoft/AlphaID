using Microsoft.EntityFrameworkCore;
using WechatWebLogin;

namespace AlphaIDEntityFramework.EntityFramework.Wechat;

public class WechatServiceProvider : IWechatServiceProvider
{
    private readonly IDSubjectsDbContext dbContext;
    private readonly DbSet<WechatService> set;

    public WechatServiceProvider(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.set = this.dbContext.Set<WechatService>();
    }

    public async Task<string?> GetSecretAsync(string appId)
    {
        var svc = await this.set.FindAsync(appId);
        return svc?.Secret;
    }

    public async Task UpdateSecretAsync(string appId, string secret)
    {
        var svc = await this.set.FindAsync(appId) ?? throw new ArgumentException("Cannot found wechat service.");
        svc.Secret = secret;
        this.dbContext.Entry(svc).State = EntityState.Modified;
        await this.dbContext.SaveChangesAsync();
    }

    public async Task RegisterAsync(string appId, string secret)
    {

        var svc = new WechatService
        {
            AppId = appId,
            Secret = secret
        };
        this.set.Add(svc);
        _ = await this.dbContext.SaveChangesAsync();
    }
}
