namespace AdminWebApp.Domain.Security;

/// <summary>
///     UserInRole Store.
/// </summary>
public interface IUserInRoleStore
{
    /// <summary>
    ///     Gets queryable user in role collection.
    /// </summary>
    IQueryable<UserInRole> UserInRoles { get; }

    /// <summary>
    ///     Create
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task CreateAsync(UserInRole item);

    /// <summary>
    ///     Delete
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task DeleteAsync(UserInRole item);
}