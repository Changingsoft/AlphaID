using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示一个附件。
/// </summary>
[PrimaryKey(nameof(DocumentId), nameof(Name))]
public class IdentityDocumentAttachment
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
    /// 附件的名称。
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 此附件的二进制数据。
    /// </summary>
    public byte[] Content { get; set; } = default!;

    /// <summary>
    /// 此附件数据的MIME类型。
    /// </summary>
    [MaxLength(100),Unicode(false)]
    public string ContentType { get; set; } = default!;
}