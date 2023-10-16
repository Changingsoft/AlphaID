using IDSubjects.Subjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects;

/// <summary>
/// 表示一个自然人个体。
/// </summary>
[Table("NaturalPerson")]
[Index(nameof(Name))]
[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(PhoneticSearchHint))]
public class NaturalPerson
{
    /// <summary>
    /// 
    /// </summary>
    protected internal NaturalPerson()
    {
        this.BankAccounts = new HashSet<PersonBankAccount>();
    }

    /// <summary>
    /// 序列号。
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [MaxLength(50)]
    [Unicode(false)]
    public string Id { get; protected internal set; } = default!;

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
    public DateTime WhenChanged { get; protected internal set; } = DateTime.UtcNow;

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
    /// LastName 或姓氏。
    /// </summary>
    [PersonalData]
    [MaxLength(10)]
    public virtual string? LastName { get; protected set; }

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
    /// 移动电话号码（全局唯一）。
    /// </summary>
    [PersonalData]
    [MaxLength(14)]
    [Unicode(false)]
    public virtual string? Mobile { get; set; }

    /// <summary>
    /// 电子邮件。
    /// </summary>
    [PersonalData]
    [MaxLength(100)]
    public virtual string? Email { get; set; }

    /// <summary>
    /// 用户名。
    /// </summary>
    [PersonalData]
    [Unicode(false)]
    [MaxLength(50)]
    public virtual string UserName { get; set; } = default!;

    /// <summary>
    /// 邮件确认。
    /// </summary>
    public virtual bool EmailConfirmed { get; set; }

    /// <summary>
    /// 密码哈希。
    /// </summary>
    [MaxLength(100)]
    [Unicode(false)]
    public virtual string? PasswordHash { get; set; }

    /// <summary>
    /// 安全戳
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public virtual string SecurityStamp { get; set; } = default!;

    /// <summary>
    /// 并发检测戳
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public virtual string? ConcurrencyStamp { get; set; }

    /// <summary>
    /// 移动电话号码是否已确认。
    /// </summary>
    public virtual bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// 是否启用双因子认证。
    /// </summary>
    public virtual bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// 锁定结束时间。
    /// </summary>
    public virtual DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// 是否启用账户锁定。
    /// </summary>
    public virtual bool LockoutEnabled { get; set; }

    /// <summary>
    /// 访问失败计数器。
    /// </summary>
    public virtual int AccessFailedCount { get; set; }

    /// <summary>
    /// 获取一个值，指示用户上一次设置密码的时间。如果该值为null，或超过设定的最大更改密码期限，则用户在登录时必须强制更改密码。
    /// </summary>
    public virtual DateTime? PasswordLastSet { get; set; }

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
