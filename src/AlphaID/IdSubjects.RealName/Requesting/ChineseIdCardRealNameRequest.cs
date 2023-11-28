using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 表示一个使用身份证的实名认证请求。
/// </summary>
public class ChineseIdCardRealNameRequest : RealNameRequest
{
    /// <summary>
    /// Ctor for persistence.
    /// </summary>
    protected ChineseIdCardRealNameRequest() { }

    /// <summary>
    /// 初始化。
    /// </summary>
    /// <param name="name">姓名。</param>
    /// <param name="sex">性别。</param>
    /// <param name="ethnicity">民族。</param>
    /// <param name="dateOfBirth">出生日期。</param>
    /// <param name="address">住址。</param>
    /// <param name="cardNumber">身份证号码。</param>
    /// <param name="issuer">签发机关。</param>
    /// <param name="issueDate">有效期起。</param>
    /// <param name="expires">有效期至。</param>
    /// <param name="personalSide">个人信息面。</param>
    /// <param name="issuerSide">签发者信息面。</param>
    public ChineseIdCardRealNameRequest(string name, Sex sex, string ethnicity, DateOnly dateOfBirth, string address, string cardNumber, string issuer, DateOnly issueDate, DateOnly? expires, BinaryDataInfo personalSide, BinaryDataInfo issuerSide)
    {
        this.Name = name;
        this.Sex = sex;
        this.Ethnicity = ethnicity;
        this.DateOfBirth = dateOfBirth;
        this.Address = address;
        this.CardNumber = cardNumber;
        this.Issuer = issuer;
        this.IssueDate = issueDate;
        this.Expires = expires;
        this.PersonalSide = personalSide;
        this.IssuerSide = issuerSide;
    }


    /// <summary>
    /// 姓名。
    /// </summary>
    [MaxLength(20)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 性别。
    /// </summary>
    public Sex Sex { get; set; }

    /// <summary>
    /// 民族。
    /// </summary>
    [MaxLength(20)]
    public string Ethnicity { get; set; } = default!;

    /// <summary>
    /// 出生日期。
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// 住址。
    /// </summary>
    [MaxLength(150)]
    public string Address { get; set; } = default!;

    /// <summary>
    /// 身份证号码。
    /// </summary>
    [MaxLength(18), Unicode(false)]
    public string CardNumber { get; set; } = default!;

    /// <summary>
    /// 签发机关。
    /// </summary>
    [MaxLength(20)]
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// 签发日期。
    /// </summary>
    public DateOnly IssueDate { get; set; } = default!;

    /// <summary>
    /// 有效期至。若为null，表示长期。
    /// </summary>
    public DateOnly? Expires { get; set; }

    /// <summary>
    /// 个人信息面。
    /// </summary>
    public BinaryDataInfo PersonalSide { get; set; } = default!;

    /// <summary>
    /// 签发者信息面。
    /// </summary>
    public BinaryDataInfo IssuerSide { get; set; } = default!;

    /// <summary>
    /// 重写，创建RealNameAuthentication。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override RealNameAuthentication CreateAuthentication()
    {
        if (!this.AcceptedAt.HasValue)
            throw new InvalidOperationException("未审核通过的请求不能创建认证信息。");
        var document = new ChineseIdCardDocument()
        {
            Address = this.Address,
            Name = this.Name,
            Sex = this.Sex,
            DateOfBirth = this.DateOfBirth,
            Ethnicity = this.Ethnicity,
            CardNumber = this.CardNumber,
            Issuer = this.Issuer,
            IssueDate = this.IssueDate,
            Expires = this.Expires
        };
        document.Attachments.Add(new IdentityDocumentAttachment(ChineseIdCardDocument.PersonalSideAttachmentName, this.PersonalSide.Data, this.PersonalSide.MimeType));
        document.Attachments.Add(new IdentityDocumentAttachment(ChineseIdCardDocument.IssuerSideAttachmentName, this.IssuerSide.Data, this.IssuerSide.MimeType));

        var authentication = new DocumentedRealNameAuthentication(document,
            new PersonNameInfo(this.Name),
            this.AcceptedAt.Value,
            this.Auditor!);
        return authentication;
    }
}
