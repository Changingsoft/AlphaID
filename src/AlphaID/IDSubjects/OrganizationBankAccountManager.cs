using System.Transactions;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
public class OrganizationBankAccountManager
{
    private readonly IOrganizationBankAccountStore store;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    public OrganizationBankAccountManager(IOrganizationBankAccountStore store)
    {
        this.store = store;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public IEnumerable<OrganizationBankAccount> GetBankAccounts(GenericOrganization organization)
    {
        return this.store.BankAccounts.Where(b => b.OrganizationId == organization.Id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public OrganizationBankAccount? GetDefault(GenericOrganization organization)
    {
        return this.store.BankAccounts.FirstOrDefault(b => b.OrganizationId == organization.Id && b.Default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="accountNumber"></param>
    /// <param name="accountName"></param>
    /// <param name="bank"></param>
    /// <param name="usage"></param>
    /// <param name="isDefault"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AddAsync(GenericOrganization organization, string accountNumber, string accountName,
        string bank, string? usage, bool isDefault = false)
    {
        if(this.store.BankAccounts.Any(b => b.OrganizationId == organization.Id && b.AccountNumber == accountNumber))
            return IdOperationResult.Failed("Bank account exists.");

        var bankAccount = new OrganizationBankAccount(organization, accountNumber, accountName, bank)
        {
            Usage = usage,
        };
        var result = await this.store.CreateAsync((bankAccount));
        if (!result.Succeeded)
            return result;

        if (isDefault)
        {
            return await this.SetDefault(bankAccount);
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> SetDefault(OrganizationBankAccount bankAccount)
    {
        if (bankAccount.Default)
            return IdOperationResult.Success;

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var currentDefault = this.GetDefault(bankAccount.Organization);
        if (currentDefault != null)
        {
            if (currentDefault == bankAccount)
                return IdOperationResult.Success;
            currentDefault.Default = false;
            await this.store.UpdateAsync(currentDefault);
        }
        bankAccount.Default = true;
        await this.store.UpdateAsync(bankAccount);
        trans.Complete();
        return IdOperationResult.Success;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> Update(OrganizationBankAccount bankAccount)
    {
        return await this.store.UpdateAsync(bankAccount);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> RemoveAsync(OrganizationBankAccount bankAccount)
    {
        return await this.store.DeleteAsync(bankAccount);
    }
}