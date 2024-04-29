using IdSubjects;
using IdSubjects.Invitations;
using IdSubjects.Payments;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework;

public class IdSubjectsDbContext(DbContextOptions<IdSubjectsDbContext> options) : DbContext(options)
{
    /// <summary>
    ///     自然人。
    /// </summary>
    public DbSet<NaturalPerson> People { get; protected set; } = default!;

    public DbSet<NaturalPersonClaim> PersonClaims { get; protected set; } = default!;

    public DbSet<NaturalPersonLogin> PersonLogins { get; protected set; } = default!;

    public DbSet<NaturalPersonToken> PersonTokens { get; protected set; } = default!;

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
}