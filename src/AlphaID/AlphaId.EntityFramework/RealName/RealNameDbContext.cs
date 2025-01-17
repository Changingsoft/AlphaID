using IdSubjects.RealName;
using IdSubjects.RealName.Requesting;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.RealName;

public class RealNameDbContext(DbContextOptions<RealNameDbContext> options) : DbContext(options)
{
    public DbSet<RealNameAuthentication> RealNameAuthentications { get; protected set; } = null!;

    public DbSet<DocumentedRealNameAuthentication> DocumentedRealNameAuthentications { get; protected set; } = null!;

    public DbSet<IdentityDocument> IdentityDocuments { get; protected set; } = null!;

    public DbSet<ChineseIdCardDocument> ChineseIdCardDocuments { get; protected set; } = null!;

    public DbSet<RealNameRequest> RealNameRequests { get; protected set; } = null!;

    public DbSet<ChineseIdCardRealNameRequest> ChineseIdCardRealNameRequests { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityDocument>().Property("Discriminator").HasMaxLength(100).IsUnicode(false);
        modelBuilder.Entity<RealNameAuthentication>().Property("Discriminator").HasMaxLength(100).IsUnicode(false);
        modelBuilder.Entity<RealNameRequest>().Property("Discriminator").HasMaxLength(100).IsUnicode(false);
    }
}