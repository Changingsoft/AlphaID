using IdSubjects;
using IdSubjects.Invitations;
using IdSubjects.Payments;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework;

public class IdSubjectsDbContext(DbContextOptions<IdSubjectsDbContext> options) : IdentityDbContext<NaturalPerson>(options)
{
    /// <summary>
    ///     自然人。
    /// </summary>
    public DbSet<NaturalPerson> People { get; protected set; } = default!;

    public DbSet<PersonBankAccount> PersonBankAccounts { get; protected set; } = default!;

    public DbSet<PasswordHistory> PasswordHistorySet { get; protected set; } = default!;

    /// <summary>
    ///     Organizations.
    /// </summary>
    public DbSet<GenericOrganization> Organizations { get; protected set; } = default!;

    public DbSet<OrganizationMember> OrganizationMembers { get; protected set; } = default!;

    public DbSet<OrganizationBankAccount> OrganizationBankAccounts { get; protected set; } = default!;

    public DbSet<OrganizationIdentifier> OrganizationIdentifiers { get; protected set; } = default!;

    public DbSet<JoinOrganizationInvitation> JoinOrganizationInvitations { get; protected set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Rename table name.
        builder.Entity<NaturalPerson>(b =>
        {
            b.ToTable("NaturalPerson");
            b.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.PasswordHash).HasMaxLength(100).IsUnicode(false);
            b.Property(p => p.SecurityStamp).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.ConcurrencyStamp).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.PhoneNumber).HasMaxLength(20).IsUnicode(false);
        });
        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.ToTable("NaturalPersonLogin");
            b.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.ProviderKey).HasMaxLength(256).IsUnicode(false);
            b.Property(p => p.ProviderDisplayName).HasMaxLength(50);
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
        });
        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.ToTable("NaturalPersonToken");
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.Name).HasMaxLength(50);
            b.Property(p => p.Value).HasMaxLength(256).IsUnicode(false);
        });
        builder.Entity<IdentityUserRole<string>>(b =>
        {
            b.ToTable("UserInRole");
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.RoleId).HasMaxLength(50).IsUnicode(false);
        });
        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.ToTable("NaturalPersonClaim");
            b.Property(p => p.ClaimType).HasMaxLength(256).IsUnicode(false);
            b.Property(p => p.ClaimValue).HasMaxLength(50);
        });

        builder.Entity<IdentityRole>(b =>
        {
            b.ToTable("Role");
            b.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.ConcurrencyStamp).HasMaxLength(50).IsUnicode(false);
        });
        builder.Entity<IdentityRoleClaim<string>>(b =>
        {
            b.ToTable("RoleClaim");
            b.Property(p => p.ClaimType).HasMaxLength(256).IsUnicode(false);
            b.Property(p => p.ClaimValue).HasMaxLength(50);
        });

    }
}