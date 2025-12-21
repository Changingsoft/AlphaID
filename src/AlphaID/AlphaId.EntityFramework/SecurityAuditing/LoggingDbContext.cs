using IdSubjects.SecurityAuditing;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.SecurityAuditing;

public class LoggingDbContext(DbContextOptions<LoggingDbContext> options) : DbContext(options)
{
    public DbSet<AuditLogEntry> AuditLog { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AuditLogEntry>(e => {
            e.ToTable("AuditLog");
            e.HasIndex(a => a.TimeStamp);
            e.Property(a => a.Level).HasMaxLength(128);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}