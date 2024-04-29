using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
[Table("PasswordHistory")]
[Index(nameof(WhenCreated))]
public class PasswordHistory
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)] public string UserId { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(100), Unicode(false)] public string Data { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset WhenCreated { get; set; }
}
