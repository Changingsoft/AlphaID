using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示一个基于证明材料的实名认证。
/// </summary>
public class DocumentedRealNameAuthentication : RealNameAuthentication
{
    /// <summary>
    /// Ctor for persistence.
    /// </summary>
    protected DocumentedRealNameAuthentication()
    {
    }

    /// <summary>
    /// 使用证明材料和相关信息初始化实名认证。
    /// </summary>
    /// <param name="document"></param>
    /// <param name="personName"></param>
    /// <param name="validatedAt"></param>
    /// <param name="validatedBy"></param>
    public DocumentedRealNameAuthentication(IdentityDocument document,
        HumanNameInfo personName,
        DateTimeOffset validatedAt,
        string validatedBy)
        : base(personName, validatedAt, validatedBy)
    {
        Document = document;
        DocumentId = document.Id;
    }

    /// <summary>
    /// 指示一份身份证明文件的Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string DocumentId { get; protected set; } = null!;

    /// <summary>
    /// 指示一个身份证明文件。
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public IdentityDocument Document { get; protected set; } = null!;
}