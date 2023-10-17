namespace AdminWebApp.Domain.Security;

/// <summary>
/// UserInRole Manager.
/// </summary>
public class UserInRoleManager
{
    private readonly IUserInRoleStore store;


    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="store"></param>
    public UserInRoleManager(IUserInRoleStore store)
    {
        this.store = store;
    }

    /// <summary>
    /// Gets roles of user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public IEnumerable<string> GetRoles(string userId)
    {
        return from userInRole in this.store.UserInRoles
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
        return from userInRole in this.store.UserInRoles
               where userInRole.RoleName == roleName
               select userInRole;
    }

    /// <summary>
    /// Add user to role.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <param name="userName"></param>
    /// <param name="userSearchHint"></param>
    /// <returns></returns>
    public async Task AddRole(string userId, string roleName, string userName, string userSearchHint)
    {
        UserInRole userInRole = new()
        {
            RoleName = roleName,
            UserId = userId,
            UserName = userName,
            UserSearchHint = userSearchHint,
        };
        await this.store.CreateAsync(userInRole);
    }

    /// <summary>
    /// Remove a user from role.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task RemoveRole(string userId, string roleName)
    {
        var userInRole = this.store.UserInRoles.FirstOrDefault(p => p.UserId == userId && p.RoleName == roleName);
        if (userInRole != null)
            await this.store.DeleteAsync(userInRole);
    }
}
