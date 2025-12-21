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
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IdentityDocument>().Property("Discriminator").HasMaxLength(100).IsUnicode(false);
        modelBuilder.Entity<RealNameAuthentication>().Property("Discriminator").HasMaxLength(100).IsUnicode(false);
        modelBuilder.Entity<RealNameRequest>(e =>
        {
            e.ToTable("RealNameRequest");
            e.Property("Discriminator").HasMaxLength(100).IsUnicode(false);
            e.HasKey(p => p.Id);
            e.Property(p => p.PersonId).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.Auditor).HasMaxLength(30);
        });
        modelBuilder.Entity<ChineseIdCardRealNameRequest>(e =>
        {
            e.OwnsOne(p => p.PersonalSide, ps =>
            {
                ps.Property(p => p.MimeType).HasMaxLength(100).IsUnicode(false);
            });
            e.OwnsOne(p => p.IssuerSide, ps =>
            {
                ps.Property(p => p.MimeType).HasMaxLength(100).IsUnicode(false);
            });
        });

        modelBuilder.Entity<IdentityDocument>(e =>
        {
            e.ToTable("IdentityDocument");
            e.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
        });
        modelBuilder.Entity<IdentityDocumentAttachment>(e =>
        {
            e.ToTable("IdentityDocumentAttachment");
            e.HasKey(p => new { p.DocumentId, p.Name });
            e.HasOne(p => p.Document).WithMany(d => d.Attachments).HasForeignKey(p => p.DocumentId);
            e.Property(p => p.Name).HasMaxLength(50);
            e.Property(p => p.ContentType).HasMaxLength(100).IsUnicode(false);
        });

        modelBuilder.Entity<ChineseIdCardDocument>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(50);
            e.Property(p => p.Sex).HasColumnType("varchar(7)");
            e.Property(p => p.Ethnicity).HasMaxLength(50);
            e.Property(p => p.Address).HasMaxLength(100);
            e.Property(p => p.CardNumber).HasMaxLength(18).IsUnicode(false);
            e.Property(p => p.Issuer).HasMaxLength(50);
        });

        modelBuilder.Entity<DocumentedRealNameAuthentication>(e =>
        {
            e.HasOne(d => d.Document).WithMany().HasForeignKey(d => d.DocumentId);
        });
        modelBuilder.Entity<RealNameAuthentication>(e =>
        {
            e.ToTable("RealNameAuthentication");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.PersonId).HasMaxLength(50).IsUnicode(false);
            e.OwnsOne(p => p.PersonName, pn =>
            {
                pn.Property(p => p.Surname).HasMaxLength(50);
                pn.Property(p => p.MiddleName).HasMaxLength(50);
                pn.Property(p => p.GivenName).HasMaxLength(50);
                pn.Property(p => p.FullName).HasMaxLength(50);
            });
            e.Property(p => p.ValidatedBy).HasMaxLength(30);
            e.Property(p => p.Remarks).HasMaxLength(200);
        });
        modelBuilder.Entity<ChineseIdCardRealNameRequest>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(50);
            e.Property(p => p.Ethnicity).HasMaxLength(50);
            e.Property(p => p.Address).HasMaxLength(150);
            e.Property(p => p.CardNumber).HasMaxLength(18).IsUnicode(false);
            e.Property(p => p.Issuer).HasMaxLength(50);
            e.OwnsOne(p => p.PersonalSide, ps =>
            {
                ps.Property(p => p.MimeType).HasMaxLength(100).IsUnicode(false);
            });
            e.OwnsOne(p => p.IssuerSide, ps =>
            {
                ps.Property(p => p.MimeType).HasMaxLength(100).IsUnicode(false);
            });
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}