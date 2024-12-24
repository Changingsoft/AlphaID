using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Domain.Security;

/// <summary>
///     表示在角色中的用户。
/// </summary>
[Table("UserInRole")]
public class UserInRole
{
    /// <summary>
    ///     用户Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string UserId { get; set; } = default!;

    /// <summary>
    ///     角色名称。
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string RoleName { get; set; } = default!;
}