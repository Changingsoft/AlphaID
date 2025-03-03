using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// </summary>
[Table("AuditLog")]
[Index(nameof(TimeStamp))]
public class AuditLogEntry
{
    /// <summary>
    /// </summary>
    [Key]
    public int Id { get; protected set; }

    /// <summary>
    /// 来源
    /// </summary>
    public string? Source { get; protected set; }

    /// <summary>
    /// 事件Id.
    /// </summary>
    public int? EventId { get; protected set; }


    /// <summary>
    /// </summary>
    public string? Message { get; protected set; }

    /// <summary>
    /// </summary>
    public string? MessageTemplate { get; protected set; }

    /// <summary>
    /// </summary>
    [MaxLength(128)]
    public string? Level { get; protected set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset TimeStamp { get; protected set; }

    /// <summary>
    /// </summary>
    public string? Exception { get; protected set; }

    /// <summary>
    /// </summary>
    public string? Properties { get; protected set; }
}