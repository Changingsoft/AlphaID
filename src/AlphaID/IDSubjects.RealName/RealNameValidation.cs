using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
public class RealNameValidation
{
    /// <summary>
    /// 
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string DocumentId { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public virtual IdentityDocument Document { get; set; } = default!;
}
