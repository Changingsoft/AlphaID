using IdSubjects.DirectoryLogon;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.DirectoryLogon.EntityFramework;

public class DirectoryLogonDbContext(DbContextOptions<DirectoryLogonDbContext> options) : DbContext(options)
{
    public DbSet<DirectoryServiceDescriptor> DirectoryServices { get; protected set; } = null!;

    public DbSet<DirectoryAccount> LogonAccounts { get; protected set; } = null!;
}