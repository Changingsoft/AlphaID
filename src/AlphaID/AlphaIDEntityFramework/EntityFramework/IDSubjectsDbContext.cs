using IDSubjects;
using IDSubjects.RealName;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AlphaIDEntityFramework.EntityFramework;

public class IDSubjectsDbContext : DbContext
{
    public IDSubjectsDbContext([NotNull] DbContextOptions<IDSubjectsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 自然人。
    /// </summary>
    public DbSet<NaturalPerson> People { get; protected set; } = default!;


    //public DbSet<IdentityRole> IdentityRoles { get; protected set; } = default!;

    //public DbSet<IdentityRoleClaim> IdentityRoleClaims { get; protected set; } = default!;

    public DbSet<NaturalPersonClaim> NaturalPersonClaims { get; protected set; } = default!;

    public DbSet<NaturalPersonLogin> NaturalPersonLogins { get; protected set; } = default!;

    public DbSet<NaturalPersonToken> NaturalPersonTokens { get; protected set; } = default!;

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
        builder.Entity<NaturalPerson>().HasIndex(p => p.Email).IsUnique(true).HasFilter(@"[Email] IS NOT NULL");
        builder.Entity<NaturalPerson>().HasIndex(p => p.Mobile).IsUnique(true).HasFilter(@"[Mobile] IS NOT NULL");
        builder.Entity<NaturalPerson>().HasIndex(p => p.WhenCreated);
        builder.Entity<NaturalPerson>().HasIndex(p => p.WhenChanged);
        builder.Entity<GenericOrganization>().HasIndex(p => p.USCI).IsUnique(true).HasFilter(@"[USCI] IS NOT NULL");
        builder.Entity<GenericOrganization>().HasIndex(p => p.WhenCreated);
        builder.Entity<GenericOrganization>().HasIndex(p => p.WhenChanged);
    }
}
