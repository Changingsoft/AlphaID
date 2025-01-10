using IdSubjects.WechatWebLogin;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.WechatWebLogin.EntityFramework;

public class WechatWebLoginDbContext(DbContextOptions<WechatWebLoginDbContext> options) : DbContext(options)
{
    public DbSet<WechatLoginSession> WechatLoginSessions { get; protected set; } = null!;

    public DbSet<WechatAppClient> WechatAppClients { get; protected set; } = null!;

    public DbSet<WechatUserIdentifier> WechatUserIdentifiers { get; protected set; } = null!;

    public DbSet<WechatService> WechatServices { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WechatAppClient>().Property(p => p.RedirectUriList).HasJsonConversion();
        modelBuilder.Entity<WechatUserIdentifier>().HasKey(p => new { p.AppId, p.OpenId });
    }
}