namespace IdSubjects.DirectoryLogon;

/// <summary>
/// 提供登录账户的可查询能力。
/// </summary>
public interface IQueryableLogonAccountStore
{
    /// <summary>
    /// 获取可查询的登录账户集合。
    /// </summary>
    IQueryable<DirectoryAccount> LogonAccounts { get; }

    /// <summary>
    /// Find logon account by logon id.
    /// </summary>
    /// <param name="logonId"></param>
    /// <returns></returns>
    Task<DirectoryAccount?> FindByLogonIdAsync(string logonId);
}
