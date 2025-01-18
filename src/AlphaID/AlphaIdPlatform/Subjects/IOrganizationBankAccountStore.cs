using IdSubjects;

namespace AlphaIdPlatform.Subjects;

/// <summary>
///     提供组织银行账户存取能力。
/// </summary>
public interface IOrganizationBankAccountStore
{
    /// <summary>
    ///     组织的银行账户。
    /// </summary>
    IQueryable<OrganizationBankAccount> BankAccounts { get; }

    /// <summary>
    ///     创建组织的银行账户
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(OrganizationBankAccount bankAccount);

    /// <summary>
    ///     更新组织的银行账户
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(OrganizationBankAccount bankAccount);

    /// <summary>
    ///     删除组织的银行账户。
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(OrganizationBankAccount bankAccount);
}