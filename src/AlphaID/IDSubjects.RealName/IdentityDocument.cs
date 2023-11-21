using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示一个身份证明文件。
/// </summary>
/// <remarks>
/// <para>
/// 身份证明文件是一种抽象，不同的身份证明文件有他们各自的属性。
/// </para>
/// <para>
/// 每一个身份证明文件均包含有一个或多个文档附件，通过<see cref="Attachments"/>检索。
/// </para>
/// </remarks>
[Table("IdentityDocument")]
public abstract class IdentityDocument
{
    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 与此身份证件关联的附件。
    /// </summary>
    public virtual ICollection<DocumentAttachment> Attachments { get; protected set; } = new HashSet<DocumentAttachment>();

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset WhenCreated { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 将实名信息应用到Person.
    /// </summary>
    /// <param name="person"></param>
    internal abstract void ApplyRealName(NaturalPerson person);
}