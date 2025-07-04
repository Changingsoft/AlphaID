using AlphaIdPlatform.Admin;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.Admin;

public class OperationalDbContext(DbContextOptions<OperationalDbContext> options) : DbContext(options)
{
    public DbSet<UserInRole> UserInRoles { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInRole>().HasKey(p => new { p.UserId, p.RoleName });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}