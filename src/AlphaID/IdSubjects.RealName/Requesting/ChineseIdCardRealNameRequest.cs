using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 
/// </summary>
public class ChineseIdCardRealNameRequest : RealNameRequest
{
    /// <summary>
    /// 
    /// </summary>
    protected ChineseIdCardRealNameRequest() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sex"></param>
    /// <param name="ethnicity"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="address"></param>
    /// <param name="cardNumber"></param>
    /// <param name="issuer"></param>
    /// <param name="issueDate"></param>
    /// <param name="expires"></param>
    /// <param name="personalSide"></param>
    /// <param name="issuerSide"></param>
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
    /// 
    /// </summary>
    public BinaryDataInfo PersonalSide { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BinaryDataInfo IssuerSide { get; set; }

    /// <summary>
    /// 
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
        document.Attachments.Add(new IdentityDocumentAttachment(nameof(this.PersonalSide), this.PersonalSide.Data, this.PersonalSide.MimeType));
        document.Attachments.Add(new IdentityDocumentAttachment(nameof(this.IssuerSide), this.IssuerSide.Data, this.IssuerSide.MimeType));

        var authentication = new DocumentedRealNameAuthentication(document,
            new PersonNameInfo(this.Name),
            this.AcceptedAt.Value,
            this.Auditor ?? "");
        return authentication;
    }

    /// <summary>
    /// 
    /// </summary>
    public DateOnly? Expires { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateOnly IssueDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CardNumber { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Ethnicity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Sex Sex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Address { get; set; }
}
