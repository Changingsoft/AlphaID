using IdSubjects.DirectoryLogon;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.DirectoryAccountManagement;

public class DirectoryLogonDbContext(DbContextOptions<DirectoryLogonDbContext> options) : DbContext(options)
{
    public DbSet<DirectoryService> DirectoryServices { get; protected set; } = null!;

    public DbSet<DirectoryAccount> LogonAccounts { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DirectoryService>(e => {
            e.ToTable("DirectoryService");
            e.Property(ds => ds.Name).HasMaxLength(50).IsUnicode(false);
            e.Property(ds => ds.Type).HasColumnType("varchar(10)");
            e.Property(p => p.ServerAddress).HasMaxLength(50);
            e.Property(p => p.RootDn).HasMaxLength(150);
            e.Property(p => p.DefaultUserAccountContainer).HasMaxLength(150);
            e.Property(p => p.UserName).HasMaxLength(50);
            e.Property(p => p.Password).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.UpnSuffix).HasMaxLength(20).IsUnicode(false);
            e.Property(p => p.SamDomainPart).HasMaxLength(10).IsUnicode(false);
        });
        modelBuilder.Entity<DirectoryAccount>(e =>
        {
            e.ToTable("LogonAccount");
            e.Property(da => da.UserId).HasMaxLength(50).IsUnicode(false);
            e.Property(da => da.ObjectId).HasMaxLength(50).IsUnicode(false);
            e.HasKey(da => new { da.ObjectId, da.ServiceId });
            e.HasOne(da => da.DirectoryService).WithMany().HasForeignKey(da => da.ServiceId);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}