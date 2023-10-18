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
public class GenericOrganization
{
    /// <summary>
    /// for persistence.
    /// </summary>
    protected internal GenericOrganization()
    {
        this.BankAccounts = new HashSet<OrganizationBankAccount>();
    }

    /// <summary>
    /// 
    /// </summary>
    public GenericOrganization(string name) : this()
    {
        this.Name = name;
        this.Id = Guid.NewGuid().ToString();
        this.Enabled = true;
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
    public string? USCI { get; protected internal set; }

    /// <summary>
    /// 创建记录的时间。
    /// </summary>
    public virtual DateTime WhenCreated { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// 记录修改的时间。
    /// </summary>
    public virtual DateTime WhenChanged { get; protected internal set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否有效。
    /// </summary>
    public virtual bool Enabled { get; protected internal set; }

    /// <summary>
    /// 银行账号。
    /// </summary>
    public virtual ICollection<OrganizationBankAccount> BankAccounts { get; protected set; } = default!;

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
    [Column(TypeName = "date")]
    public virtual DateTime? EstablishedAt { get; set; }

    /// <summary>
    /// 营业期起
    /// </summary>
    [Column(TypeName = "date")]
    public virtual DateTime? TermBegin { get; set; }

    /// <summary>
    /// 营业期止。
    /// </summary>
    [Column(TypeName = "date")]
    public virtual DateTime? TermEnd { get; set; }

    /// <summary>
    /// 法定代表人名称。
    /// </summary>
    [MaxLength(20)]
    public virtual string? LegalPersonName { get; set; }

    /// <summary>
    /// 标示该组织的地理位置。
    /// </summary>
    [Column(TypeName = "geography")]
    public virtual Geometry? Location { get; set; } = default!;

    /// <summary>
    /// 曾用名。
    /// </summary>
    public virtual ICollection<OrganizationUsedName> UsedNames { get; protected set; } = default!;
}
