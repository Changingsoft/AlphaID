using IdSubjects;
using IdSubjects.Invitations;
using IdSubjects.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AlphaId.EntityFramework;

public class IdSubjectsDbContext : DbContext
{
    public IdSubjectsDbContext(DbContextOptions<IdSubjectsDbContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>()
                .HaveColumnType("date");

        configurationBuilder.Properties<DateOnly?>()
            .HaveConversion<NullableDateOnlyConverter>()
            .HaveColumnType("date");
    }

    /// <summary>
    /// 自然人。
    /// </summary>
    public DbSet<NaturalPerson> People { get; protected set; } = default!;

    public DbSet<NaturalPersonClaim> PersonClaims { get; protected set; } = default!;

    public DbSet<NaturalPersonLogin> PersonLogins { get; protected set; } = default!;

    public DbSet<NaturalPersonToken> PersonTokens { get; protected set; } = default!;

    public DbSet<PersonBankAccount> PersonBankAccounts { get; protected set; } = default!;

    public DbSet<PasswordHistory> PasswordHistorySet { get; protected set; } = default!;

    /// <summary>
    /// Organizations.
    /// </summary>
    public DbSet<GenericOrganization> Organizations { get; protected set; } = default!;

    public DbSet<OrganizationMember> OrganizationMembers { get; protected set; } = default!;

    public DbSet<OrganizationBankAccount> OrganizationBankAccounts { get; protected set; } = default!;

    public DbSet<OrganizationIdentifier> OrganizationIdentifiers { get; protected set; } = default!;

    public DbSet<JoinOrganizationInvitation> JoinOrganizationInvitations { get; protected set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    /// <summary>
    /// Converts <see cref="DateOnly" /> to <see cref="DateTime"/> and vice versa.
    /// </summary>
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public DateOnlyConverter() : base(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d))
        { }
    }

    /// <summary>
    /// Compares <see cref="DateOnly" />.
    /// </summary>
    public class DateOnlyComparer : ValueComparer<DateOnly>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public DateOnlyComparer() : base(
            (d1, d2) => d1 == d2 && d1.DayNumber == d2.DayNumber,
            d => d.GetHashCode())
        {
        }
    }

    /// <summary>
    /// Converts <see cref="DateOnly" /> to <see cref="DateTime"/> and vice versa.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Local
    private class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public NullableDateOnlyConverter() : base(
            d => d == null
                ? null
                : new DateTime?(d.Value.ToDateTime(TimeOnly.MinValue)),
            d => d == null
                ? null
                : new DateOnly?(DateOnly.FromDateTime(d.Value)))
        { }
    }

    /// <summary>
    /// Compares <see cref="DateOnly" />.
    /// </summary>
    public class NullableDateOnlyComparer : ValueComparer<DateOnly?>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public NullableDateOnlyComparer() : base(
            (d1, d2) => d1 == d2 && d1.GetValueOrDefault().DayNumber == d2.GetValueOrDefault().DayNumber,
            d => d.GetHashCode())
        {
        }
    }
}
