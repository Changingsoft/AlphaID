using WechatWebLogin;

namespace AlphaIDEntityFramework.EntityFramework.Wechat;

public class WechatUserIdentifierStore : IWechatUserIdentifierStore
{
    private readonly WechatWebLoginDbContext dbContext;

    public WechatUserIdentifierStore(WechatWebLoginDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<WechatUserIdentifier> WechatUserIdentifiers => this.dbContext.WechatUserIdentifiers;

    public async Task CreateAsync(WechatUserIdentifier item)
    {
        this.dbContext.WechatUserIdentifiers.Add(item);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(WechatUserIdentifier item)
    {
        this.dbContext.WechatUserIdentifiers.Remove(item);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<WechatUserIdentifier?> FindAsync(string wxAppId, string openId)
    {
        return await this.dbContext.WechatUserIdentifiers.FindAsync(wxAppId, openId);
    }

    public async Task UpdateAsync(WechatUserIdentifier item)
    {
        this.dbContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
    }
}
