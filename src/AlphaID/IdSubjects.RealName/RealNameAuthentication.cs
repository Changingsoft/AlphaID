using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.RealName;

/// <summary>
/// 表示一个实名认证。
/// </summary>
[Table("RealNameAuthentication")]
public abstract class RealNameAuthentication
{
    /// <summary>
    /// </summary>
    protected RealNameAuthentication()
    {
    }

    /// <summary>
    /// 初始化实名认证信息。
    /// </summary>
    /// <param name="personName"></param>
    /// <param name="validatedAt"></param>
    /// <param name="validatedBy"></param>
    protected RealNameAuthentication(HumanNameInfo personName, DateTimeOffset validatedAt, string validatedBy)
    {
        ValidatedAt = validatedAt;
        ValidatedBy = validatedBy;
        PersonName = personName;
    }

    /// <summary>
    /// Id。
    /// </summary>
    [Key]
    [MaxLength(50)]
    [Unicode(false)]
    public string Id { get; protected set; } = Guid.NewGuid().ToString().ToLower();

    /// <summary>
    /// 所属的自然人Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string PersonId { get; protected internal set; } = null!;

    /// <summary>
    /// 与此实名认证有关的个人名称信息。
    /// </summary>
    public HumanNameInfo PersonName { get; protected set; } = null!;

    /// <summary>
    /// 认证通过的时间。
    /// </summary>
    public DateTimeOffset ValidatedAt { get; protected set; }

    /// <summary>
    /// 认证执行者。
    /// </summary>
    [MaxLength(30)]
    public string ValidatedBy { get; protected set; } = null!;

    /// <summary>
    /// 有效期限。
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; protected set; }

    /// <summary>
    /// 备注和其他信息
    /// </summary>
    [MaxLength(200)]
    public string? Remarks { get; set; }

    /// <summary>
    /// 将实名认证信息应用到自然人。
    /// </summary>
    /// <param name="person"></param>
    public virtual void ApplyToPerson(ApplicationUser person)
    {
        person.FamilyName = PersonName.Surname;
        person.GivenName = PersonName.GivenName;
        person.MiddleName = PersonName.MiddleName;
        person.Name = PersonName.FullName;
    }
}