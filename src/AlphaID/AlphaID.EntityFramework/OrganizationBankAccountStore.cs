using IdSubjects;

namespace AlphaId.EntityFramework;

internal class OrganizationBankAccountStore : IOrganizationBankAccountStore
{
    private readonly IdSubjectsDbContext dbContext;

    public OrganizationBankAccountStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<OrganizationBankAccount> BankAccounts => this.dbContext.OrganizationBankAccounts;

    public async Task<IdOperationResult> CreateAsync(OrganizationBankAccount bankAccount)
    {
        this.dbContext.OrganizationBankAccounts.Add(bankAccount);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(OrganizationBankAccount bankAccount)
    {
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(OrganizationBankAccount bankAccount)
    {
        this.dbContext.OrganizationBankAccounts.Remove(bankAccount);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
