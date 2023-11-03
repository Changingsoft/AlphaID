using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects;

/// <summary>
/// 组织。
/// </summary>
[Table("Organization")]
[Index(nameof(Name))]
[Index(nameof(WhenCreated))]
[Index(nameof(WhenChanged))]
public class GenericOrganization
{
    /// <summary>
    /// for persistence.
    /// </summary>
    protected internal GenericOrganization()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public GenericOrganization(string name) : this()
    {
        this.Name = name;
    }

    /// <summary>
    /// 序列号。
    /// </summary>
    [Key]
    [MaxLength(50), Unicode(false)]
    public string Id { get; protected internal set; } = Guid.NewGuid().ToString().ToLower();

    /// <summary>
    /// Name.
    /// </summary>
    [MaxLength(100)]
    public virtual string Name { get; protected internal set; } = default!;

    /// <summary>
    /// 统一社会信用代码。
    /// </summary>
    [MaxLength(18), Column(TypeName = "char(18)")]
    public string? USCI { get; set; }

    /// <summary>
    /// 创建记录的时间。
    /// </summary>
    public virtual DateTimeOffset WhenCreated { get; protected internal set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 记录修改的时间。
    /// </summary>
    public virtual DateTimeOffset WhenChanged { get; protected internal set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 是否有效。
    /// </summary>
    public virtual bool Enabled { get; protected internal set; } = true;

    /// <summary>
    /// 银行账号。
    /// </summary>
    public virtual ICollection<OrganizationBankAccount> BankAccounts { get; protected set; } = new HashSet<OrganizationBankAccount>();

    /// <summary>
    /// 住所。
    /// </summary>
    [MaxLength(100)]
    public virtual string? Domicile { get; set; }

    /// <summary>
    /// 联系方式。
    /// </summary>
    [MaxLength(50)]
    public virtual string? Contact { get; set; }


    /// <summary>
    /// 注册时间。
    /// </summary>
    public virtual DateOnly? EstablishedAt { get; set; }

    /// <summary>
    /// 营业期起
    /// </summary>
    public virtual DateOnly? TermBegin { get; set; }

    /// <summary>
    /// 营业期止。
    /// </summary>
    public virtual DateOnly? TermEnd { get; set; }

    /// <summary>
    /// 组织的代表人。
    /// </summary>
    [MaxLength(20)]
    public virtual string? Representative { get; set; }

    /// <summary>
    /// 标示该组织的地理位置。
    /// </summary>
    [Column(TypeName = "geography")]
    public virtual Geometry? Location { get; set; }

    /// <summary>
    /// 组织的网站。
    /// </summary>
    [MaxLength(256)]
    public virtual string? Website { get; set; }

    /// <summary>
    /// 曾用名。
    /// </summary>
    public virtual ICollection<OrganizationUsedName> UsedNames { get; protected set; } = new HashSet<OrganizationUsedName>();

    /// <summary>
    /// Description of organization.
    /// </summary>
    [MaxLength(200)]
    public virtual string? Description { get; set; }
}
