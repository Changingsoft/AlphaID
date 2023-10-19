using Microsoft.EntityFrameworkCore;
using WechatWebLogin;

namespace AlphaIDEntityFramework.EntityFramework.Wechat;

public class WechatLoginSessionStore : IWechatLoginSessionStore
{
    private readonly WechatWebLoginDbContext dbContext;

    public WechatLoginSessionStore(WechatWebLoginDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CleanExpiredSessionsAsync()
    {
        var expires = this.dbContext.WechatLoginSessions.Where(p => p.WhenExpires < DateTime.UtcNow).ToArray();
        this.dbContext.WechatLoginSessions.RemoveRange(expires);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task CreateAsync(WechatLoginSession session)
    {
        this.dbContext.WechatLoginSessions.Add(session);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<WechatLoginSession?> FindAsync(string sessionId)
    {
        return await this.dbContext.WechatLoginSessions.FindAsync(sessionId);
    }

    public async Task UpdateAsync(WechatLoginSession session)
    {
        this.dbContext.Entry(session).State = EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
    }
}
