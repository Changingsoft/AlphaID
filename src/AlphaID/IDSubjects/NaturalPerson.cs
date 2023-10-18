using IDSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace IDSubjects;

/// <summary>
/// 表示一个自然人个体。
/// </summary>
[Table("NaturalPerson")]
[Index(nameof(Name))]
[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(PhoneticSearchHint))]
[Index(nameof(WhenCreated))]
[Index(nameof(WhenChanged))]
public class NaturalPerson : IdentityUser
{
    /// <summary>
    /// 
    /// </summary>
    public NaturalPerson():base()
    {
        this.BankAccounts = new HashSet<PersonBankAccount>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    public NaturalPerson(string userName):base(userName)
    {
        this.BankAccounts = new HashSet<PersonBankAccount>();
    }

    /// <summary>
    /// Name.
    /// </summary>
    [PersonalData]
    [MaxLength(20)]
    public string Name { get; protected internal set; } = default!;

    /// <summary>
    /// When Created.
    /// </summary>
    public DateTime WhenCreated { get; protected internal set; } = DateTime.UtcNow;

    /// <summary>
    /// When Changed.
    /// </summary>
    public DateTime WhenChanged { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 启用或禁用该自然人。如果禁用，自然人不会出现在一般搜索结果中。但可以通过Id查询。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// First Name 或姓名中的名字。
    /// </summary>
    [PersonalData]
    [MaxLength(10)]
    public virtual string? FirstName { get; protected set; }

    /// <summary>
    /// Middle name.
    /// </summary>
    [PersonalData]
    [MaxLength(50)]
    public virtual string? MiddleName { get; set; }

    /// <summary>
    /// LastName 或姓氏。
    /// </summary>
    [PersonalData]
    [MaxLength(10)]
    public virtual string? LastName { get; protected set; }

    /// <summary>
    /// 昵称。
    /// </summary>
    [PersonalData]
    [MaxLength(20)]
    public virtual string? NickName { get; set; }

    /// <summary>
    /// 姓氏拼音
    /// </summary>
    [MaxLength(20), Unicode(false)]
    public virtual string? PhoneticSurname { get; set; }

    /// <summary>
    /// 名字拼音
    /// </summary>
    [MaxLength(40), Unicode(false)]
    public virtual string? PhoneticGivenName { get; set; }

    /// <summary>
    /// 读音检索提示。（即去掉空格的读音名字）
    /// </summary>
    [Unicode(false)]
    [MaxLength(60)]
    public virtual string? PhoneticSearchHint { get; set; }

    /// <summary>
    /// 性别。
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    [Comment("性别")]
    public virtual Sex? Sex { get; set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    [Column(TypeName = "date")]
    public virtual DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// 获取一个值，指示用户上一次设置密码的时间。如果该值为null，或超过设定的最大更改密码期限，则用户在登录时必须强制更改密码。
    /// </summary>
    public virtual DateTime? PasswordLastSet { get; set; }

    /// <summary>
    /// User head image data.
    /// </summary>
    public virtual BinaryDataInfo? Avatar { get; set; }

    /// <summary>
    /// 区域和语言选项
    /// </summary>
    [MaxLength(10),Unicode(false)]
    public virtual string? Locale { get; set; }

    /// <summary>
    /// 用户所选择的时区。存储为IANA Time zone database名称。
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public virtual string? TimeZone { get; protected internal set; }

    /// <summary>
    /// 地址。
    /// </summary>
    public virtual AddressInfo? Address { get; set; }

    /// <summary>
    /// 个人主页。
    /// </summary>
    public virtual string? WebSite { get; set; }
    /// <summary>
    /// Gets bank accounts of the person.
    /// </summary>
    public virtual ICollection<PersonBankAccount> BankAccounts { get; protected set; } = default!;


    /// <summary>
    /// Overrided.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{this.UserName}|{this.Name}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chinesePersonName"></param>
    public void SetName(ChinesePersonName chinesePersonName)
    {
        this.Name = chinesePersonName.FullName;
        this.FirstName = chinesePersonName.GivenName;
        this.LastName = chinesePersonName.Surname;
        this.PhoneticSurname = chinesePersonName.PhoneticSurname;
        this.PhoneticGivenName = chinesePersonName.PhoneticGivenName;
        this.PhoneticSearchHint = chinesePersonName.PhoneticName.Replace(" ", string.Empty);
    }
}
