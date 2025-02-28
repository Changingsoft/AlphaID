namespace AdminWebApp.Domain.Security;

/// <summary>
/// 角色常量。
/// </summary>
public class RoleConstants
{
    static RoleConstants()
    {
        var roles = new HashSet<Role>
        {
            new() { Name = "Administrators", DisplayName = "系统管理员" },
            new() { Name = "ApplicationAdmin", DisplayName = "应用程序管理员" }
        };
        Roles = roles;
    }

    /// <summary>
    /// 获取角色列表。
    /// </summary>
    public static IEnumerable<Role> Roles { get; }

    /// <summary>
    /// 具有完全管理权的角色。
    /// </summary>
    public static Role AdministratorsRole => Roles.First(p => p.Name == "Administrators");

    /// <summary>
    /// 应用程序管理角色。
    /// </summary>
    public static Role ApplicationAdminRole => Roles.First(p => p.Name == "ApplicationAdmin");
}