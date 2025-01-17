using IdSubjects;

namespace AlphaId.EntityFramework.IdSubjects;

internal class OrganizationBankAccountStore(IdSubjectsDbContext dbContext) : IOrganizationBankAccountStore
{
    public IQueryable<OrganizationBankAccount> BankAccounts => dbContext.OrganizationBankAccounts;

    public async Task<IdOperationResult> CreateAsync(OrganizationBankAccount bankAccount)
    {
        dbContext.OrganizationBankAccounts.Add(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(OrganizationBankAccount bankAccount)
    {
        dbContext.OrganizationBankAccounts.Update(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(OrganizationBankAccount bankAccount)
    {
        dbContext.OrganizationBankAccounts.Remove(bankAccount);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}