using IdSubjects.WechatWebLogin;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.WechatWebLogin.EntityFramework;

public class WechatLoginSessionStore(WechatWebLoginDbContext dbContext) : IWechatLoginSessionStore
{
    public async Task CleanExpiredSessionsAsync()
    {
        var expires = dbContext.WechatLoginSessions.Where(p => p.WhenExpires < DateTime.UtcNow).ToArray();
        dbContext.WechatLoginSessions.RemoveRange(expires);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task CreateAsync(WechatLoginSession session)
    {
        dbContext.WechatLoginSessions.Add(session);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<WechatLoginSession?> FindAsync(string sessionId)
    {
        return await dbContext.WechatLoginSessions.FindAsync(sessionId);
    }

    public async Task UpdateAsync(WechatLoginSession session)
    {
        dbContext.Entry(session).State = EntityState.Modified;
        _ = await dbContext.SaveChangesAsync();
    }
}
