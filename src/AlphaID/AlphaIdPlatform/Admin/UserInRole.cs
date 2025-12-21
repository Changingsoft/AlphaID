
namespace AlphaIdPlatform.Admin;

/// <summary>
/// 表示在角色中的用户。
/// </summary>
public class UserInRole
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 角色名称。
    /// </summary>
    public string RoleName { get; set; } = null!;
}