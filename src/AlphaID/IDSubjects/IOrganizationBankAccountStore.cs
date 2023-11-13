namespace IDSubjects;

/// <summary>
/// 
/// </summary>
public interface IOrganizationBankAccountStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<OrganizationBankAccount> BankAccounts { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(OrganizationBankAccount bankAccount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(OrganizationBankAccount bankAccount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(OrganizationBankAccount bankAccount);
}