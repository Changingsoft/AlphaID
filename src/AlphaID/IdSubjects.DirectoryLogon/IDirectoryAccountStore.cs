namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Provide store for LogonAccount.
/// </summary>
public interface IDirectoryAccountStore
{
    /// <summary>
    /// Gets queryable account collection.
    /// </summary>
    IQueryable<DirectoryAccount> Accounts { get; }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task CreateAsync(DirectoryAccount account);

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task UpdateAsync(DirectoryAccount account);

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task DeleteAsync(DirectoryAccount account);
}
