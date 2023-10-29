using DirectoryLogon;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AlphaID.DirectoryLogon.EntityFramework;

public class DirectoryLogonDbContext : DbContext
{
    public DirectoryLogonDbContext([NotNull] DbContextOptions<DirectoryLogonDbContext> options) : base(options)
    {
    }

    public DbSet<DirectoryService> DirectoryServices { get; protected set; } = default!;

    public DbSet<LogonAccount> LogonAccounts { get; protected set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogonAccount>().Property(p => p.LogonId).UseCollation("Chinese_PRC_CS_AS");
    }
}
