using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
[PrimaryKey(nameof(DocumentId), nameof(Name))]
public class DocumentAttachment
{
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

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public byte[] Content { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(100),Unicode(false)]
    public string ContentType { get; set; } = default!;
}