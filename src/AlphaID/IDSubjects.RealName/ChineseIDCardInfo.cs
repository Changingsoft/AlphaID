using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects.RealName;

/// <summary>
/// 身份证信息
/// </summary>
[Owned]
public class ChineseIDCardInfo
{
    /// <summary>
    /// 
    /// </summary>
    protected ChineseIDCardInfo() { }

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
    public ChineseIDCardInfo(string name,
                         Sex sex,
                         string ethnicity,
                         DateTime dateOfBirth,
                         string address,
                         string cardNumber,
                         string issuer,
                         DateTime issueDate,
                         DateTime? expires = null)
    {
        this.Name = name;
        this.Ethnicity = ethnicity;
        this.Address = address;
        this.CardNumber = cardNumber;
        this.Sex = sex;
        this.DateOfBirth = dateOfBirth;
        this.Issuer = issuer;
        this.IssueDate = issueDate;
        this.Expires = expires;
    }

    /// <summary>
    /// 姓名
    /// </summary>
    [MaxLength(50)]
    public string Name { get; protected set; } = default!;

    /// <summary>
    /// 性别
    /// </summary>
    [Column(TypeName = "varchar(7)")]
    public Sex Sex { get; protected set; }

    /// <summary>
    /// 民族
    /// </summary>
    [MaxLength(50)]
    public string Ethnicity { get; protected set; } = default!;

    /// <summary>
    /// 出生日期
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime DateOfBirth { get; protected set; }

    /// <summary>
    /// 住址
    /// </summary>
    [MaxLength(100)]
    public string Address { get; protected set; } = default!;

    /// <summary>
    /// 身份证号
    /// </summary>
    [MaxLength(18), Unicode(false)]
    public string CardNumber { get; protected set; } = default!;

    /// <summary>
    /// 签发机关
    /// </summary>
    [MaxLength(50)]
    public string Issuer { get; protected set; } = default!;

    /// <summary>
    /// 有效期起
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime IssueDate { get; protected set; }

    /// <summary>
    /// 有效期至
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? Expires { get; protected set; }
}
