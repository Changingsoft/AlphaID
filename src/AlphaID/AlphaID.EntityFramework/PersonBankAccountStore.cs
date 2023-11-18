using IdSubjects;
using IdSubjects.Payments;

namespace AlphaId.EntityFramework;

internal class PersonBankAccountStore : IPersonBankAccountStore
{
    private readonly IdSubjectsDbContext dbContext;

    public PersonBankAccountStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<PersonBankAccount> BankAccounts => this.dbContext.PersonBankAccounts;

    public async Task<IdOperationResult> CreateAsync(PersonBankAccount bankAccount)
    {
        this.dbContext.PersonBankAccounts.Add(bankAccount);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(PersonBankAccount bankAccount)
    {
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(PersonBankAccount bankAccount)
    {
        this.dbContext.PersonBankAccounts.Remove(bankAccount);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
