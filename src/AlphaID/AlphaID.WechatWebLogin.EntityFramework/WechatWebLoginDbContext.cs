using IdSubjects.WechatWebLogin;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.WechatWebLogin.EntityFramework;
public class WechatWebLoginDbContext : DbContext
{
    public WechatWebLoginDbContext(DbContextOptions<WechatWebLoginDbContext> options) : base(options)
    {
    }

    public DbSet<WechatLoginSession> WechatLoginSessions { get; protected set; } = default!;

    public DbSet<WechatAppClient> WechatAppClients { get; protected set; } = default!;

    public DbSet<WechatUserIdentifier> WechatUserIdentifiers { get; protected set; } = default!;

    public DbSet<WechatService> WechatServices { get; protected set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WechatAppClient>().Property(p => p.RedirectUriList).HasJsonConversion();
        modelBuilder.Entity<WechatUserIdentifier>().HasKey(p => new { p.AppId, p.OpenId });
    }
}
