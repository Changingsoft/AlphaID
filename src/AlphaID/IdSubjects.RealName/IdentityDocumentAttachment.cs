
namespace IdSubjects.RealName;

/// <summary>
/// 表示一个附件。
/// </summary>
public class IdentityDocumentAttachment
{
    /// <summary>
    /// Ctor for persistence.
    /// </summary>
    protected IdentityDocumentAttachment()
    {
    }

    /// <summary>
    /// 初始化一个附件。
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
    /// 该附件指向的证明材料Id.
    /// </summary>
    public string DocumentId { get; set; } = null!;

    /// <summary>
    /// 此附件所属的证明材料。
    /// </summary>
    public virtual IdentityDocument Document { get; set; } = null!;

    /// <summary>
    /// 附件的名称。
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 此附件的二进制数据。
    /// </summary>
    public byte[] Content { get; set; } = null!;

    /// <summary>
    /// 此附件数据的MIME类型。
    /// </summary>
    public string ContentType { get; set; } = null!;
}