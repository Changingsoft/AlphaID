namespace AlphaIdPlatform.Admin;

/// <summary>
/// UserInRole Manager.
/// </summary>
/// <remarks>
/// Ctor.
/// </remarks>
/// <param name="store"></param>
public class UserInRoleManager(IUserInRoleStore store)
{
    /// <summary>
    /// Gets roles of user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public IEnumerable<string> GetRoles(string userId)
    {
        return from userInRole in store.UserInRoles
               where userInRole.UserId == userId
               select userInRole.RoleName;
    }

    /// <summary>
    /// Gets users in role.
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public IEnumerable<UserInRole> GetUserInRoles(string roleName)
    {
        return from userInRole in store.UserInRoles
               where userInRole.RoleName == roleName
               select userInRole;
    }

    /// <summary>
    /// Add user to role.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public Task AddRole(string userId, string roleName)
    {
        if (store.UserInRoles.Any(p => p.UserId == userId && p.RoleName == roleName))
            return Task.CompletedTask;
        UserInRole userInRole = new()
        {
            RoleName = roleName,
            UserId = userId
        };
        return store.CreateAsync(userInRole);
    }

    /// <summary>
    /// Remove a user from role.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task RemoveRole(string userId, string roleName)
    {
        UserInRole? userInRole = store.UserInRoles.FirstOrDefault(p => p.UserId == userId && p.RoleName == roleName);
        if (userInRole != null)
            await store.DeleteAsync(userInRole);
    }
}