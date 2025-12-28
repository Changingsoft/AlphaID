using NetTopologySuite.Geometries;

namespace Organizational;

/// <summary>
/// 组织。
/// </summary>
public class Organization
{
    /// <summary>
    /// for persistence.
    /// </summary>
    protected internal Organization()
    {
    }

    /// <summary>
    /// </summary>
    public Organization(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Id。
    /// </summary>
    public string Id { get; protected internal set; } = Guid.NewGuid().ToString().ToLower();

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; protected set; } = null!;

    /// <summary>
    /// 住所。
    /// </summary>
    public virtual string? Domicile { get; set; }

    /// <summary>
    /// 联系方式。
    /// </summary>
    public virtual string? Contact { get; set; }

    /// <summary>
    /// 公开电子邮件。
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 组织的代表人。
    /// </summary>
    public virtual string? Representative { get; set; }

    /// <summary>
    /// 统一社会信用代码。
    /// </summary>
    public string? USCC { get; set; }

    /// <summary>
    /// 邓白氏码。
    /// </summary>
    public string? DUNS { get; set; }

    /// <summary>
    /// 法人实体标识码。
    /// </summary>
    public string? LEI { get; set; }

    /// <summary>
    /// 组织的头像。
    /// </summary>
    public virtual BinaryDataInfo? ProfilePicture { get; set; }

    /// <summary>
    /// 创建记录的时间。
    /// </summary>
    public virtual DateTimeOffset WhenCreated { get; protected internal set; }

    /// <summary>
    /// 记录修改的时间。
    /// </summary>
    public virtual DateTimeOffset WhenChanged { get; protected internal set; }

    /// <summary>
    /// 是否有效。
    /// </summary>
    public virtual bool Enabled { get; protected internal set; } = true;

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
    /// 标示该组织的地理位置。
    /// </summary>
    public virtual Geometry? Location { get; set; }

    /// <summary>
    /// 组织的网站。
    /// </summary>
    public virtual string? Website { get; set; }

    /// <summary>
    /// Description of organization.
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// 发票信息。
    /// </summary>
    public virtual FapiaoInfo? Fapiao { get; set; }

    /// <summary>
    /// 曾用名。
    /// </summary>
    public virtual ICollection<OrganizationUsedName> UsedNames { get; protected set; } = [];

    /// <summary>
    ///    银行账号。
    /// </summary>
    public virtual ICollection<OrganizationBankAccount> BankAccounts { get; protected set; } = [];

    /// <summary>
    /// 组织标识。
    /// </summary>
    [Obsolete("不再考虑使用。")]
    public virtual ICollection<OrganizationIdentifier> OrganizationIdentifiers { get; protected set; } = [];

    /// <summary>
    /// 组织的成员。
    /// </summary>
    public virtual ICollection<OrganizationMember> Members { get; protected set; } = [];
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Name;
    }

    internal void SetName(string newName, bool recordUsedName, DateOnly changeDate)
    {
        if(recordUsedName)
        {
            UsedNames.Add(new OrganizationUsedName() { Name = this.Name, DeprecateTime = changeDate});
        }
        Name = newName;
    }
}