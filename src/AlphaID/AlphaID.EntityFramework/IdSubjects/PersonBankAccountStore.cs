using IdSubjects;
using IdSubjects.Payments;

namespace AlphaId.EntityFramework.IdSubjects;

internal class PersonBankAccountStore(IdSubjectsDbContext dbContext) : IPersonBankAccountStore
{
    public IQueryable<PersonBankAccount> BankAccounts => dbContext.PersonBankAccounts;

    public async Task<IdOperationResult> CreateAsync(PersonBankAccount bankAccount)
    {
        dbContext.PersonBankAccounts.Add(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(PersonBankAccount bankAccount)
    {
        dbContext.PersonBankAccounts.Update(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(PersonBankAccount bankAccount)
    {
        dbContext.PersonBankAccounts.Remove(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}