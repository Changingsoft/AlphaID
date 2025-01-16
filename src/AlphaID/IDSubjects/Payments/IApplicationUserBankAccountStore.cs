namespace IdSubjects.Payments;

/// <summary>
///     提供持久化个人银行账户的能力。
/// </summary>
public interface IApplicationUserBankAccountStore
{
    /// <summary>
    ///     获取银行账户集合。
    /// </summary>
    IQueryable<ApplicationUserBankAccount> BankAccounts { get; }

    /// <summary>
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(ApplicationUserBankAccount bankAccount);

    /// <summary>
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(ApplicationUserBankAccount bankAccount);

    /// <summary>
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(ApplicationUserBankAccount bankAccount);
}