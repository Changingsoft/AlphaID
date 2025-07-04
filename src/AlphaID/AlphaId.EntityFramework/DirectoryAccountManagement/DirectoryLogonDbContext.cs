using IdSubjects.DirectoryLogon;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.DirectoryAccountManagement;

public class DirectoryLogonDbContext(DbContextOptions<DirectoryLogonDbContext> options) : DbContext(options)
{
    public DbSet<DirectoryService> DirectoryServices { get; protected set; } = null!;

    public DbSet<DirectoryAccount> LogonAccounts { get; protected set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}