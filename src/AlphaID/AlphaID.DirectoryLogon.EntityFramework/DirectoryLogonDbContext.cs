using IdSubjects.DirectoryLogon;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.DirectoryLogon.EntityFramework;

public class DirectoryLogonDbContext(DbContextOptions<DirectoryLogonDbContext> options) : DbContext(options)
{
    public DbSet<DirectoryServiceDescriptor> DirectoryServices { get; protected set; } = default!;

    public DbSet<DirectoryAccount> LogonAccounts { get; protected set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
