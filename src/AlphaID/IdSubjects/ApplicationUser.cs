using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects;

/// <summary>
/// 表示一个用户。
/// </summary>
[Index(nameof(WhenCreated))]
[Index(nameof(WhenChanged))]
[Index(nameof(Name))]
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// for persistence.
    /// </summary>
    public ApplicationUser()
    {
    }

    /// <summary>
    /// 使用用户名创建一个新的用户。
    /// </summary>
    /// <param name="userName"></param>
    public ApplicationUser(string userName) : base(userName)
    {
    }


    /// <summary>
    /// 获取一个值，指示用户上一次设置密码的时间。如果该值为null，或超过设定的最大更改密码期限，则用户在登录时必须强制更改密码。
    /// </summary>
    public virtual DateTimeOffset? PasswordLastSet { get; protected internal set; }


    /// <summary>
    /// When Created.
    /// </summary>
    public virtual DateTimeOffset WhenCreated { get; protected internal set; }

    /// <summary>
    /// When Changed.
    /// </summary>
    public virtual DateTimeOffset WhenChanged { get; set; }

    /// <summary>
    /// 启用或禁用该自然人。如果禁用，用户将无法登录。
    /// </summary>
    public virtual bool Enabled { get; set; } = true;

    /// <summary>
    ///    姓。
    /// </summary>
    [MaxLength(50)]
    [PersonalData]
    public string? FamilyName { get; set; }

    /// <summary>
    ///    中间名。
    /// </summary>
    [MaxLength(50)]
    [PersonalData]
    public string? MiddleName { get; set; }

    /// <summary>
    ///    名。
    /// </summary>
    [MaxLength(50)]
    [PersonalData]
    public string? GivenName { get; set; }

    /// <summary>
    /// 全名。
    /// </summary>
    [MaxLength(50)]
    [PersonalData]
    public string? Name { get; set; }

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
    [PersonalData]
    public virtual Gender? Gender { get; set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    [PersonalData]
    public virtual DateOnly? DateOfBirth { get; set; }


    /// <summary>
    /// User head image data.
    /// </summary>
    public virtual BinaryDataInfo? ProfilePicture { get; set; }

    /// <summary>
    /// 区域和语言选项
    /// </summary>
    [MaxLength(10)]
    [Unicode(false)]
    [PersonalData]
    public virtual string? Locale { get; protected internal set; }

    /// <summary>
    /// 用户所选择的时区。存储为IANA Time zone database名称。
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    [PersonalData]
    public virtual string? TimeZone { get; protected internal set; }

    /// <summary>
    /// 地址。
    /// </summary>
    [PersonalData]
    public virtual AddressInfo? Address { get; set; }

    /// <summary>
    /// 个人主页。
    /// </summary>
    [MaxLength(256)]
    [PersonalData]
    public virtual string? WebSite { get; set; }

    /// <summary>
    /// 密码历史记录。
    /// </summary>
    public virtual ICollection<UsedPassword> UsedPasswords { get; set; } = [];

    /// <inheritdoc />
    public override string ToString()
    {
        return UserName!;
    }
}