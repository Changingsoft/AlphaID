namespace DirectoryLogon;

/// <summary>
/// Provide store for LogonAccount.
/// </summary>
public interface ILogonAccountStore
{
    /// <summary>
    /// Gets queryable account collection.
    /// </summary>
    IQueryable<LogonAccount> Accounts { get; }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task CreateAsync(LogonAccount account);

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task UpdateAsync(LogonAccount account);

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task DeleteAsync(LogonAccount account);
}
