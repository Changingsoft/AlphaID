using AlphaIDPlatform.Security;
using Microsoft.EntityFrameworkCore;

namespace OperationalEF;

public class OperationalDbContext : DbContext
{
    public OperationalDbContext(DbContextOptions<OperationalDbContext> options) : base(options)
    {
    }

    public DbSet<UserInRole> UserInRoles { get; protected set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInRole>().HasKey(p => new { p.UserId, p.RoleName });
    }
}
