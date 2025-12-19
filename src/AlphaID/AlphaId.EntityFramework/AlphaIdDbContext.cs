using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.JoinOrgRequesting;
using AlphaIdPlatform.Subjects;
using Microsoft.EntityFrameworkCore;

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
            e.Property(p => p.USCC).HasMaxLength(18).IsUnicode(false);
            e.Property(p => p.DUNS).HasMaxLength(9).IsUnicode(false);
            e.Property(p => p.LEI).HasMaxLength(20).IsUnicode(false);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}
