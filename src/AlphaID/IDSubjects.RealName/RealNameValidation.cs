using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示一个实名认证。
/// </summary>
[Table("RealNameValidation")]
public class RealNameValidation
{
    protected RealNameValidation() { }

    public RealNameValidation(IdentityDocument document, DateTimeOffset validatedAt, string validatedBy)
    {
        this.Document = document;
        this.ValidatedAt = validatedAt;
        this.ValidatedBy = validatedBy;
    }

    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string DocumentId { get; protected set; } = default!;

    /// <summary>
    /// 指示一个身份证明材料。
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public virtual IdentityDocument Document { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset ValidatedAt { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public string ValidatedBy { get; protected set; } = default!;
}
