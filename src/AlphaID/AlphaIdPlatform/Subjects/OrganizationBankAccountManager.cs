using System.Transactions;
using IdSubjects;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="store"></param>
public class OrganizationBankAccountManager(IOrganizationBankAccountStore store)
{
    /// <summary>
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public IEnumerable<OrganizationBankAccount> GetBankAccounts(Organization organization)
    {
        return store.BankAccounts.Where(b => b.OrganizationId == organization.Id);
    }

    /// <summary>
    /// </summary>
    /// <param name="organization"></param>
    /// <returns></returns>
    public OrganizationBankAccount? GetDefault(Organization organization)
    {
        return store.BankAccounts.FirstOrDefault(b => b.OrganizationId == organization.Id && b.Default);
    }

    /// <summary>
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="accountNumber"></param>
    /// <param name="accountName"></param>
    /// <param name="bank"></param>
    /// <param name="usage"></param>
    /// <param name="isDefault"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AddAsync(Organization organization,
        string accountNumber,
        string accountName,
        string bank,
        string? usage,
        bool isDefault = false)
    {
        if (store.BankAccounts.Any(b => b.OrganizationId == organization.Id && b.AccountNumber == accountNumber))
            return IdOperationResult.Failed("Bank account exists.");

        var bankAccount = new OrganizationBankAccount(organization, accountNumber, accountName, bank)
        {
            Usage = usage
        };
        IdOperationResult result = await store.CreateAsync(bankAccount);
        if (!result.Succeeded)
            return result;

        if (isDefault) return await SetDefault(bankAccount);

        return result;
    }

    /// <summary>
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> SetDefault(OrganizationBankAccount bankAccount)
    {
        if (bankAccount.Default)
            return IdOperationResult.Success;

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        OrganizationBankAccount? currentDefault = GetDefault(bankAccount.Organization);
        if (currentDefault != null)
        {
            if (currentDefault == bankAccount)
                return IdOperationResult.Success;
            currentDefault.Default = false;
            await store.UpdateAsync(currentDefault);
        }

        bankAccount.Default = true;
        await store.UpdateAsync(bankAccount);
        trans.Complete();
        return IdOperationResult.Success;
    }

    /// <summary>
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    public Task<IdOperationResult> Update(OrganizationBankAccount bankAccount)
    {
        return store.UpdateAsync(bankAccount);
    }

    /// <summary>
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    public Task<IdOperationResult> RemoveAsync(OrganizationBankAccount bankAccount)
    {
        return store.DeleteAsync(bankAccount);
    }
}