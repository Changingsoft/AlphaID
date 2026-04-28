using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.JoinOrgRequesting;
using Microsoft.EntityFrameworkCore;
using Organizational;

namespace AlphaId.EntityFramework;
public class AlphaIdDbContext(DbContextOptions<AlphaIdDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Organizations.
    /// </summary>
    public DbSet<Organization> Organizations { get; protected set; } = null!;

    public DbSet<JoinOrganizationInvitation> JoinOrganizationInvitations { get; protected set; } = null!;

    public DbSet<JoinOrganizationRequest> JoinOrganizationRequests { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Organization>(e =>
        {
            e.ToTable("Organization");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.Name).HasMaxLength(100);
            e.Property(p => p.Domicile).HasMaxLength(100);
            e.Property(p => p.Contact).HasMaxLength(50);
            e.Property(p => p.Email).HasMaxLength(100).IsUnicode(false);
            e.Property(p => p.Representative).HasMaxLength(20);
            e.Property(p => p.USCC).HasMaxLength(18).IsUnicode(false);
            e.Property(p => p.DUNS).HasMaxLength(9).IsUnicode(false);
            e.Property(p => p.LEI).HasMaxLength(20).IsUnicode(false);
            e.OwnsOne(p => p.ProfilePicture, pp =>
            {
                pp.Property(p => p.MimeType).HasMaxLength(100).IsUnicode(false);
            });
            e.Property(p => p.Location).HasColumnType("geography");
            e.Property(p => p.Website).HasMaxLength(256);
            e.Property(p => p.Description).HasMaxLength(200);
            e.OwnsOne(p => p.Fapiao, f =>
            {
                f.Property(p => p.Name).HasMaxLength(100);
                f.Property(p => p.TaxPayerId).HasMaxLength(20).IsUnicode(false);
                f.Property(p => p.Address).HasMaxLength(200);
                f.Property(p => p.Contact).HasMaxLength(50);
                f.Property(p => p.Bank).HasMaxLength(100);
                f.Property(p => p.Account).HasMaxLength(50).IsUnicode(false);
            });
            e.HasMany(p => p.UsedNames).WithOne().HasForeignKey(p => p.OrganizationId);
            e.HasMany(p => p.BankAccounts).WithOne().HasForeignKey(p => p.OrganizationId);
            e.HasMany(p => p.Members).WithOne().HasForeignKey(p => p.OrganizationId);
            
            e.HasIndex(p => p.Name);
            e.HasIndex(p => p.USCC).IsUnique();
            e.HasIndex(p => p.WhenCreated);
            e.HasIndex(p => p.WhenChanged);
        });

        modelBuilder.Entity<OrganizationUsedName>(e =>
        {
            e.ToTable("OrganizationUsedName");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(100);
        });
        modelBuilder.Entity<OrganizationBankAccount>(e =>
        {
            e.ToTable("OrganizationBankAccount");
            e.HasKey(p => new { p.OrganizationId, p.AccountNumber });
            e.Property(p => p.AccountNumber).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.AccountName).HasMaxLength(100);
            e.Property(p => p.BankName).HasMaxLength(100);
            e.Property(p => p.Usage).HasMaxLength(20);
        });
        modelBuilder.Entity<OrganizationMember>(e =>
        {
            e.ToTable("OrganizationMember");
            e.HasKey(p => new { p.OrganizationId, p.PersonId });
            e.Property(p => p.PersonId).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.Department).HasMaxLength(50);
            e.Property(p => p.Title).HasMaxLength(50);
            e.Property(p => p.Remark).HasMaxLength(50);
            e.HasIndex(p => p.PersonId);
        });

        modelBuilder.Entity<JoinOrganizationInvitation>(e =>
        {
            e.ToTable("JoinOrganizationInvitation");
            e.HasKey(p => p.Id);
            e.Property(p => p.InviteeId).HasMaxLength(50);
            e.Property(p => p.OrganizationId).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.Inviter).HasMaxLength(50);
        });
        modelBuilder.Entity<JoinOrganizationRequest>(e =>
        {
            e.ToTable("JoinOrganizationRequest");
            e.HasKey(p => p.Id);
            e.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.OrganizationName).HasMaxLength(50);
            e.Property(p => p.OrganizationId).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.AuditBy).HasMaxLength(50);
            e.HasIndex(p => p.WhenCreated);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLazyLoadingProxies();
    }
}
