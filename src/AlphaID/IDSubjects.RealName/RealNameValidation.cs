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
    /// <summary>
    /// 
    /// </summary>
    protected RealNameValidation() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <param name="personName"></param>
    /// <param name="validatedAt"></param>
    /// <param name="validatedBy"></param>
    public RealNameValidation(IdentityDocument document, PersonNameInfo personName, DateTimeOffset validatedAt, string validatedBy)
    {
        this.Document = document;
        this.ValidatedAt = validatedAt;
        this.ValidatedBy = validatedBy;
        this.PersonName = personName;
    }

    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; protected set; }

    /// <summary>
    /// 指示一份身份证明文件的Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string DocumentId { get; protected set; } = default!;

    /// <summary>
    /// 指示一个身份证明文件。
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public virtual IdentityDocument Document { get; protected set; } = default!;

    /// <summary>
    /// 与此实名认证有关的个人名称信息。
    /// </summary>
    public PersonNameInfo PersonName { get;protected set; } = default!;

    /// <summary>
    /// 认证通过的时间。
    /// </summary>
    public DateTimeOffset ValidatedAt { get; protected set; }

    /// <summary>
    /// 认证执行者。
    /// </summary>
    public string ValidatedBy { get; protected set; } = default!;

    /// <summary>
    /// 有效期限。
    /// </summary>
    public DateTimeOffset? ExpiresAt {  get; protected set; }

    /// <summary>
    /// 备注和其他信息
    /// </summary>
    [MaxLength(200)]
    public string? Remark { get; set; }
}
