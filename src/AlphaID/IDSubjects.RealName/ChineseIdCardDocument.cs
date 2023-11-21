using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示中华人民共和国居民身份证。
/// </summary>
public class ChineseIdCardDocument : IdentityDocument
{
    /// <summary>
    /// 姓名。
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 性别。
    /// </summary>
    [Column(TypeName = "varchar(7)")]
    public Sex Sex { get; set; }

    /// <summary>
    /// 民族。
    /// </summary>
    [MaxLength(50)]
    public string Ethnicity { get; set; } = default!;

    /// <summary>
    /// 出生日期。
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// 住址。
    /// </summary>
    [MaxLength(100)]
    public string Address { get; set; } = default!;

    /// <summary>
    /// 身份证号码。
    /// </summary>
    [MaxLength(18), Unicode(false)]
    public string CardNumber { get; set; } = default!;

    /// <summary>
    /// 签发机关。
    /// </summary>
    [MaxLength(50)]
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// 有效期起始。
    /// </summary>
    public DateOnly IssueDate { get; set; }

    /// <summary>
    /// 有效期至。如果为null，表示长期有效。
    /// </summary>
    public DateOnly? Expires { get; set; }

    internal override void ApplyRealName(NaturalPerson person)
    {
        //todo 应用时要考虑姓氏和名字拆分？
        var personName = new PersonNameInfo(this.Name);
        person.PersonName = personName;
    }
}
