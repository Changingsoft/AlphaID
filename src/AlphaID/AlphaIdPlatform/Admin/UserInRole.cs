using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaIdPlatform.Admin;

/// <summary>
/// 表示在角色中的用户。
/// </summary>
[Table("AppUserInRole")]
public class UserInRole
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 角色名称。
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string RoleName { get; set; } = null!;
}