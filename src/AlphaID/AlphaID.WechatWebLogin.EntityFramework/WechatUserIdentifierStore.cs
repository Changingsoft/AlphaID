using IdSubjects.WechatWebLogin;

namespace AlphaId.WechatWebLogin.EntityFramework;

public class WechatUserIdentifierStore(WechatWebLoginDbContext dbContext) : IWechatUserIdentifierStore
{
    public IQueryable<WechatUserIdentifier> WechatUserIdentifiers => dbContext.WechatUserIdentifiers;

    public async Task CreateAsync(WechatUserIdentifier item)
    {
        dbContext.WechatUserIdentifiers.Add(item);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(WechatUserIdentifier item)
    {
        dbContext.WechatUserIdentifiers.Remove(item);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<WechatUserIdentifier?> FindAsync(string wxAppId, string openId)
    {
        return await dbContext.WechatUserIdentifiers.FindAsync(wxAppId, openId);
    }

    public async Task UpdateAsync(WechatUserIdentifier item)
    {
        dbContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await dbContext.SaveChangesAsync();
    }
}
