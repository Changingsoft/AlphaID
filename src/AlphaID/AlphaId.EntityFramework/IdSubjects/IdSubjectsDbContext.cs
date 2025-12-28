using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

public class IdSubjectsDbContext<T>(DbContextOptions options) : IdentityDbContext<T>(options)
    where T : ApplicationUser
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Rename table name.
        builder.Entity<T>(b =>
        {
            b.ToTable("ApplicationUser");
            b.Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.PasswordHash).HasMaxLength(255).IsUnicode(false);
            b.Property(p => p.SecurityStamp).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.ConcurrencyStamp).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.PhoneNumber).HasMaxLength(20).IsUnicode(false);
            b.HasIndex(p => p.UserName).IsUnique();
            b.HasIndex(p => p.Email);
            b.HasIndex(p => p.WhenCreated);
            b.HasIndex(p => p.WhenChanged);
            b.HasIndex(p => p.Name);
            b.Property(p => p.Gender).HasColumnType("varchar(6)").HasComment("性别");
            b.Property(p => p.NickName).HasMaxLength(20);
            b.Property(p => p.Name).HasMaxLength(50);
            b.Property(p => p.MiddleName).HasMaxLength(50);
            b.Property(p => p.FamilyName).HasMaxLength(50);
            b.Property(p => p.GivenName).HasMaxLength(50);
            b.Property(p => p.Locale).HasMaxLength(10).IsUnicode(false);
            b.Property(p => p.TimeZone).HasMaxLength(50).IsUnicode(false);
            b.Property(p => p.WebSite).HasMaxLength(256);

            var picture = b.OwnsOne(a => a.ProfilePicture);
            picture.Property(p => p.MimeType).HasMaxLength(100).IsUnicode(false);

            var owned = b.OwnsOne(a => a.Address);
            owned.Property(p => p.Country).HasMaxLength(50);
            owned.Property(p => p.Region).HasMaxLength(50);
            owned.Property(p => p.Locality).HasMaxLength(50);
            owned.Property(p => p.Street1).HasMaxLength(50);
            owned.Property(p => p.Street2).HasMaxLength(50);
            owned.Property(p => p.Street3).HasMaxLength(50);
            owned.Property(p => p.Recipient).HasMaxLength(50);
            owned.Property(p => p.Contact).HasMaxLength(20).IsUnicode(false);
            owned.Property(p => p.PostalCode).HasMaxLength(20).IsUnicode(false);

            var usedPasswords = b.OwnsMany(a => a.UsedPasswords);
            usedPasswords.ToTable("UsedPassword");
            usedPasswords.Property(p => p.PasswordHash).HasMaxLength(255).IsUnicode(false);
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