using AlphaIdPlatform.Admin;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.Admin;

public class OperationalDbContext(DbContextOptions<OperationalDbContext> options) : DbContext(options)
{
    public DbSet<UserInRole> UserInRoles { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInRole>(e =>
        {
            e.ToTable("AppUserInRole");
            e.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.RoleName).HasMaxLength(50).IsUnicode(false);
            e.HasKey(p => new { p.UserId, p.RoleName });
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}