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

    public DbSet<OrganizationUsedName> OrganizationUsedNames { get; protected set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<NaturalPerson>(e =>
        {
            e.ToTable("NaturalPerson");
            e.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.PasswordHash).HasMaxLength(100).IsUnicode(false);
            e.Property(p => p.SecurityStamp).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.ConcurrencyStamp).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.PhoneNumber).HasMaxLength(20).IsUnicode(false);
        });

        builder.Entity<IdentityUserLogin<string>>(e =>
        {
            e.ToTable("UserExternalLogin");
            e.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.ProviderKey).HasMaxLength(256).IsUnicode(false);
            e.Property(p => p.ProviderDisplayName).HasMaxLength(50);
            e.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
        });
        builder.Entity<IdentityUserToken<string>>(e =>
        {
            e.ToTable("UserToken");
            e.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.Name).HasMaxLength(50);
            e.Property(p => p.Value).HasMaxLength(256).IsUnicode(false);

        });
        builder.Entity<IdentityUserClaim<string>>(e =>
        {
            e.ToTable("UserClaim");
            e.Property(p => p.ClaimType).HasMaxLength(256).IsUnicode(false);
            e.Property(p => p.ClaimValue).HasMaxLength(50);
        });

        builder.Entity<GenericOrganization>(e =>
        {
            e.HasIndex(p => p.USCI).IsUnique(true).HasFilter(@"[USCI] IS NOT NULL");
        });
    }
}
