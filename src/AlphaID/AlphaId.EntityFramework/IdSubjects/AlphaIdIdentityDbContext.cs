using AlphaIdPlatform.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;
public class AlphaIdIdentityDbContext(DbContextOptions<AlphaIdIdentityDbContext> options) : IdSubjectsDbContext<NaturalPerson>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<NaturalPerson>(e =>
        {
            e.Property(p => p.Bio).HasMaxLength(200);
            e.Property(p => p.PhoneticSurname).HasMaxLength(20).IsUnicode(false);
            e.Property(p => p.PhoneticGivenName).HasMaxLength(40).IsUnicode(false);
            e.Property(p => p.SearchHint).HasMaxLength(60);
            e.HasMany(p => p.BankAccounts).WithOne().HasForeignKey(p => p.NaturalPersonId);
            e.HasIndex(p => p.SearchHint);
        });
        builder.Entity<NaturalPersonBankAccount>(e =>
        {
            e.ToTable("NaturalPersonBankAccount");
            e.HasKey(p => new { p.NaturalPersonId, p.AccountNumber });
            e.Property(p => p.AccountNumber).HasMaxLength(50).IsUnicode(false);
            e.Property(p => p.BankName).HasMaxLength(100);
            e.Property(p => p.AccountName).HasMaxLength(100);
        });
    }
}
