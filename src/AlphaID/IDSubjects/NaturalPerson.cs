using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects;

/// <summary>
/// 表示一个自然人个体。
/// </summary>
[Table("NaturalPerson")]
[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(NormalizedUserName), IsUnique = true)]
[Index(nameof(WhenCreated))]
[Index(nameof(WhenChanged))]
public class NaturalPerson
{
    /// <summary>
    /// for persistence.
    /// </summary>
    protected NaturalPerson()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="personName"></param>
    public NaturalPerson(string userName, PersonNameInfo personName) : this()
    {
        // ReSharper disable VirtualMemberCallInConstructor
        this.UserName = userName;
        this.PersonName = personName;
        // ReSharper restore VirtualMemberCallInConstructor
    }

    /// <summary>
    /// Primary Id
    /// </summary>
    [Key]
    [MaxLength(50), Unicode(false)]
    public string Id { get; protected set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// User Name
    /// </summary>
    [MaxLength(256)]
    public virtual string UserName { get; protected internal set; } = default!;

    /// <summary>
    /// Normalized user name.
    /// </summary>
    [MaxLength(256)]
    public virtual string NormalizedUserName { get; protected internal set; } = default!;

    /// <summary>
    /// Gets or sets the email address for this user.
    /// </summary>
    [ProtectedPersonalData]
    [MaxLength(256)]
    public virtual string? Email { get; protected internal set; }

    /// <summary>
    /// Gets or sets the normalized email address for this user.
    /// </summary>
    [MaxLength(256)]
    public virtual string? NormalizedEmail { get; protected internal set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their email address.
    /// </summary>
    /// <value>True if the email address has been confirmed, otherwise false.</value>
    [PersonalData]
    public virtual bool EmailConfirmed { get; protected internal set; }

    /// <summary>
    /// Gets or sets a telephone number for the user.
    /// </summary>
    [ProtectedPersonalData]
    [MaxLength(20), Unicode(false)]
    public virtual string? PhoneNumber { get; protected internal set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their telephone address.
    /// </summary>
    /// <value>True if the telephone number has been confirmed, otherwise false.</value>
    [PersonalData]
    public virtual bool PhoneNumberConfirmed { get; protected internal set; }

    /// <summary>
    /// Gets or sets a salted and hashed representation of the password for this user.
    /// </summary>
    [MaxLength(100), Unicode(false)]
    public virtual string? PasswordHash { get; protected internal set; }

    /// <summary>
    /// 获取一个值，指示用户上一次设置密码的时间。如果该值为null，或超过设定的最大更改密码期限，则用户在登录时必须强制更改密码。
    /// </summary>
    public virtual DateTimeOffset? PasswordLastSet { get; protected internal set; }

    /// <summary>
    /// A random value that must change whenever a users credentials change (password changed, login removed)
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public virtual string? SecurityStamp { get; protected internal set; }

    /// <summary>
    /// A random value that must change whenever a user is persisted to the store
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets a flag indicating if two-factor authentication is enabled for this user.
    /// </summary>
    /// <value>True if 2fa is enabled, otherwise false.</value>
    [PersonalData]
    public virtual bool TwoFactorEnabled { get; protected internal set; }

    /// <summary>
    /// Gets or sets a flag indicating if the user could be locked out.
    /// </summary>
    /// <value>True if the user could be locked out, otherwise false.</value>
    public virtual bool LockoutEnabled { get; protected internal set; }

    /// <summary>
    /// Gets or sets the number of failed login attempts for the current user.
    /// </summary>
    public virtual int AccessFailedCount { get; protected internal set; }

    /// <summary>
    /// Gets or sets the date and time, in UTC, when any user lockout ends.
    /// </summary>
    /// <remarks>
    /// A value in the past means the user is not locked out.
    /// </remarks>
    public virtual DateTimeOffset? LockoutEnd { get; protected internal set; }

    /// <summary>
    /// When Created.
    /// </summary>
    public virtual DateTimeOffset WhenCreated { get; protected internal set; }

    /// <summary>
    /// When Changed.
    /// </summary>
    public virtual DateTimeOffset WhenChanged { get; set; }

    /// <summary>
    /// 获取有关自然人更新的时间。
    /// </summary>
    public virtual DateTimeOffset PersonWhenChanged { get; protected internal set; }

    /// <summary>
    /// 启用或禁用该自然人。如果禁用，自然人不会出现在一般搜索结果中。但可以通过Id查询。
    /// </summary>
    public virtual bool Enabled { get; set; } = true;

    /// <summary>
    /// 用户名称
    /// </summary>
    public virtual PersonNameInfo PersonName { get; set; } = default!;

    /// <summary>
    /// 昵称。
    /// </summary>
    [PersonalData]
    [MaxLength(20)]
    public virtual string? NickName { get; set; }

    /// <summary>
    /// 性别。
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    [Comment("性别")]
    public virtual Gender? Gender { get; set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    public virtual DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// 个人经历。
    /// </summary>
    [MaxLength(200)]
    public virtual string? Bio { get; set; }


    /// <summary>
    /// 姓氏拼音
    /// </summary>
    [PersonalData]
    [MaxLength(20), Unicode(false)]
    public virtual string? PhoneticSurname { get; set; }

    /// <summary>
    /// 名字拼音
    /// </summary>
    [PersonalData]
    [MaxLength(40), Unicode(false)]
    public virtual string? PhoneticGivenName { get; set; }

    /// <summary>
    /// User head image data.
    /// </summary>
    public virtual BinaryDataInfo? ProfilePicture { get; set; }

    /// <summary>
    /// 区域和语言选项
    /// </summary>
    [MaxLength(10), Unicode(false)]
    public virtual string? Locale { get; protected internal set; }

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
    [MaxLength(256)]
    public virtual string? WebSite { get; set; }

    /// <summary>
    /// Override.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{this.UserName}|{this.PersonName.FullName}";
    }
}
