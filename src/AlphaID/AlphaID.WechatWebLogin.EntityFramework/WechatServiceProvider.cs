using IdSubjects.WechatWebLogin;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.WechatWebLogin.EntityFramework;

public class WechatServiceProvider : IWechatServiceProvider
{
    private readonly WechatWebLoginDbContext _dbContext;
    private readonly DbSet<WechatService> _set;

    public WechatServiceProvider(WechatWebLoginDbContext dbContext)
    {
        _dbContext = dbContext;
        _set = _dbContext.WechatServices;
    }

    public async Task<string?> GetSecretAsync(string appId)
    {
        WechatService? svc = await _set.FindAsync(appId);
        return svc?.Secret;
    }

    public async Task UpdateSecretAsync(string appId, string secret)
    {
        WechatService svc = await _set.FindAsync(appId) ?? throw new ArgumentException("Cannot found wechat service.");
        svc.Secret = secret;
        _dbContext.Entry(svc).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RegisterAsync(string appId, string secret)
    {
        var svc = new WechatService
        {
            AppId = appId,
            Secret = secret
        };
        _set.Add(svc);
        _ = await _dbContext.SaveChangesAsync();
    }
}