using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AlphaIDEntityFramework.EntityFramework;

public class IDSubjectsDbContext : IdentityUserContext<NaturalPerson>
{
    public IDSubjectsDbContext([NotNull] DbContextOptions<IDSubjectsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 自然人。
    /// </summary>
    public DbSet<NaturalPerson> People { get; protected set; } = default!;


    public DbSet<ChineseIDCardValidation> RealNameValidations { get; protected set; } = default!;

    /// <summary>
    /// Organizations.
    /// </summary>
    public DbSet<GenericOrganization> Organizations { get; protected set; } = default!;

    public DbSet<OrganizationMember> OrganizationMembers { get; protected set; } = default!;

    public DbSet<OrganizationAdministrator> OrganizationAdministrators { get; protected set; } = default!;


    public DbSet<OrganizationUsedName> OrganizationUsedNames { get; protected set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
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
            b.ToTable("UserExternalLogin");
            b.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.ProviderKey).HasMaxLength(256).IsUnicode(false);
            b.Property(p => p.ProviderDisplayName).HasMaxLength(50);
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
        });
        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.ToTable("UserToken");
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.Name).HasMaxLength(50);
            b.Property(p => p.Value).HasMaxLength(256).IsUnicode(false);

        });
        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.ToTable("UserClaim");
            b.Property(p => p.ClaimType).HasMaxLength(256).IsUnicode(false);
            b.Property(p => p.ClaimValue).HasMaxLength(50);
        });

        builder.Entity<GenericOrganization>(e =>
        {
            e.HasIndex(p => p.USCI).IsUnique(true).HasFilter(@"[USCI] IS NOT NULL");
            //e.HasIndex(p => p.WhenCreated);
            //e.HasIndex(p => p.WhenChanged);
        });
    }
}
