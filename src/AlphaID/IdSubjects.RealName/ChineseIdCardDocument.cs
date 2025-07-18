using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示中华人民共和国居民身份证文档。
/// </summary>
public class ChineseIdCardDocument : IdentityDocument
{
    /// <summary>
    /// 身份证个人信息面名称。
    /// </summary>
    public static readonly string PersonalSideAttachmentName = "个人信息面";

    /// <summary>
    /// 身份证签发者信息面名称。
    /// </summary>
    public static readonly string IssuerSideAttachmentName = "签发者信息面";

    /// <summary>
    /// 姓名。
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 性别。
    /// </summary>
    [Column(TypeName = "varchar(7)")]
    public Sex Sex { get; set; }

    /// <summary>
    /// 民族。
    /// </summary>
    [MaxLength(50)]
    public string Ethnicity { get; set; } = null!;

    /// <summary>
    /// 出生日期。
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// 住址。
    /// </summary>
    [MaxLength(100)]
    public string Address { get; set; } = null!;

    /// <summary>
    /// 身份证号码。
    /// </summary>
    [MaxLength(18)]
    [Unicode(false)]
    public string CardNumber { get; set; } = null!;

    /// <summary>
    /// 签发机关。
    /// </summary>
    [MaxLength(50)]
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// 有效期起始。
    /// </summary>
    public DateOnly IssueDate { get; set; }

    /// <summary>
    /// 有效期至。如果为null，表示长期有效。
    /// </summary>
    public DateOnly? Expires { get; set; }

    /// <summary>
    /// 获取身份证个人信息面附件。
    /// </summary>
    [NotMapped]
    public IdentityDocumentAttachment? PersonalSide
    {
        get { return Attachments.FirstOrDefault(a => a.Name == PersonalSideAttachmentName); }
    }

    /// <summary>
    /// 获取身份证签发者信息面附件。
    /// </summary>
    public IdentityDocumentAttachment? IssuerSide
    {
        get { return Attachments.FirstOrDefault(a => a.Name == IssuerSideAttachmentName); }
    }

    internal override void ApplyRealName(ApplicationUser person)
    {
        //todo 应用时要考虑姓氏和名字拆分？
        person.Name = Name;
    }
}