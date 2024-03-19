using AdminWebApp.Domain.Security;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Infrastructure.DataStores;

public class OperationalDbContext(DbContextOptions<OperationalDbContext> options) : DbContext(options)
{
    public DbSet<UserInRole> UserInRoles { get; protected set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInRole>().HasKey(p => new { p.UserId, p.RoleName });
    }
}
