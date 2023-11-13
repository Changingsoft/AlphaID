namespace IDSubjects.Payments;
/// <summary>
/// 提供持久化个人银行账户的能力。
/// </summary>
public interface IPersonBankAccountStore
{
    /// <summary>
    /// 获取银行账户集合。
    /// </summary>
    IQueryable<PersonBankAccount> BankAccounts { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(PersonBankAccount bankAccount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(PersonBankAccount bankAccount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(PersonBankAccount bankAccount);
}
