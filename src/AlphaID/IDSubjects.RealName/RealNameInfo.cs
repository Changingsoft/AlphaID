using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects.RealName;

/// <summary>
/// 
/// </summary>
[Table("RealNameInfo")]
public class RealNameInfo
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public string PersonId { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(PersonId))]
    public virtual NaturalPerson Person { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset AcceptedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string AcceptedBy { get; set; } = default!;
}