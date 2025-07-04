using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

public class IdSubjectsDbContext(DbContextOptions<IdSubjectsDbContext> options) : IdentityDbContext<NaturalPerson>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Rename table name.
        builder.Entity<NaturalPerson>(b =>
        {
            b.ToTable("ApplicationUser");
            b.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.PasswordHash).HasMaxLength(255).IsUnicode(false);
            b.Property(p => p.SecurityStamp).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.ConcurrencyStamp).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.PhoneNumber).HasMaxLength(20).IsUnicode(false);
            b.HasIndex(p => p.UserName).IsUnique();
            b.HasIndex(p => p.Email);
        });
        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.ToTable("ApplicationUserLogin");
            b.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.ProviderKey).HasMaxLength(256).IsUnicode(false);
            b.Property(p => p.ProviderDisplayName).HasMaxLength(50);
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
        });
        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.ToTable("ApplicationUserToken");
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.LoginProvider).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.Name).HasMaxLength(50);
            b.Property(p => p.Value).HasMaxLength(256).IsUnicode(false);
        });
        builder.Entity<IdentityUserRole<string>>(b =>
        {
            b.ToTable("ApplicationUserInRole");
            b.Property(p => p.UserId).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.RoleId).HasMaxLength(50).IsUnicode(false);
        });
        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.ToTable("ApplicationUserClaim");
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