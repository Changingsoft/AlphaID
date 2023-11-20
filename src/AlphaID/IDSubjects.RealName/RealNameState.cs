using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
[Table("RealNameState")]
public class RealNameState
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    [MaxLength(50), Unicode(false)]
    public string Id { get; protected set; } = default!;

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
    [MaxLength(50)]
    public string AcceptedBy { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<RealNameValidation> Validations { get; protected set; } =
        new HashSet<RealNameValidation>();

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "varchar(15)")]
    public ActionIndicator ActionIndicator { get; set; }
}