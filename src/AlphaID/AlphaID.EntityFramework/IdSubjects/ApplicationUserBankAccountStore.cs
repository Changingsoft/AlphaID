using IdSubjects;
using IdSubjects.Payments;

namespace AlphaId.EntityFramework.IdSubjects;

internal class ApplicationUserBankAccountStore(IdSubjectsDbContext dbContext) : IApplicationUserBankAccountStore
{
    public IQueryable<ApplicationUserBankAccount> BankAccounts => dbContext.PersonBankAccounts;

    public async Task<IdOperationResult> CreateAsync(ApplicationUserBankAccount bankAccount)
    {
        dbContext.PersonBankAccounts.Add(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(ApplicationUserBankAccount bankAccount)
    {
        dbContext.PersonBankAccounts.Update(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(ApplicationUserBankAccount bankAccount)
    {
        dbContext.PersonBankAccounts.Remove(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}