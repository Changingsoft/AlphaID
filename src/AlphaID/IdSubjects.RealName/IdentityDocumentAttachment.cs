using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.RealName;

/// <summary>
///     表示一个附件。
/// </summary>
[PrimaryKey(nameof(DocumentId), nameof(Name))]
public class IdentityDocumentAttachment
{
    /// <summary>
    ///     Ctor for persistence.
    /// </summary>
    protected IdentityDocumentAttachment()
    {
    }

    /// <summary>
    ///     初始化一个附件。
    /// </summary>
    /// <param name="name">名称。</param>
    /// <param name="content">二进制内容。</param>
    /// <param name="contentType">内容的MIME类型。</param>
    public IdentityDocumentAttachment(string name, byte[] content, string contentType)
    {
        Name = name;
        Content = content;
        ContentType = contentType;
    }

    /// <summary>
    ///     该附件指向的证明材料Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string DocumentId { get; set; } = null!;

    /// <summary>
    ///     此附件所属的证明材料。
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public virtual IdentityDocument Document { get; set; } = null!;

    /// <summary>
    ///     附件的名称。
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     此附件的二进制数据。
    /// </summary>
    public byte[] Content { get; set; } = null!;

    /// <summary>
    ///     此附件数据的MIME类型。
    /// </summary>
    [MaxLength(100)]
    [Unicode(false)]
    public string ContentType { get; set; } = null!;
}