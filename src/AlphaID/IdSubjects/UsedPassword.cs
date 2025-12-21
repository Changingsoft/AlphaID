using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects;

/// <summary>
/// 表示一个已使用的密码记录。
/// </summary>
public class UsedPassword
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 已使用的密码哈希值。
    /// </summary>
    public string PasswordHash { get; set; } = null!;
}
