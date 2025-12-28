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
            e.OwnsMany(p => p.BankAccounts, ba =>
            {
                ba.ToTable("NaturalPersonBankAccount");
                ba.HasKey(p => new { p.NaturalPersonId, p.AccountNumber });
                ba.WithOwner().HasForeignKey(p => p.NaturalPersonId);
                ba.Property(p => p.AccountNumber).HasMaxLength(50).IsUnicode(false);
                ba.Property(p => p.BankName).HasMaxLength(100);
            });
            e.HasIndex(p => p.SearchHint);
        });
    }
}
