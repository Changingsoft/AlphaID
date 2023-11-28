using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示一个实名认证。
/// </summary>
[Table("RealNameAuthentication")]
public abstract class RealNameAuthentication
{
    /// <summary>
    /// 
    /// </summary>
    protected RealNameAuthentication() { }

    /// <summary>
    /// 初始化实名认证信息。
    /// </summary>
    /// <param name="personName"></param>
    /// <param name="validatedAt"></param>
    /// <param name="validatedBy"></param>
    protected RealNameAuthentication(PersonNameInfo personName, DateTimeOffset validatedAt, string validatedBy)
    {
        this.ValidatedAt = validatedAt;
        this.ValidatedBy = validatedBy;
        this.PersonName = personName;
    }

    /// <summary>
    /// Id。
    /// </summary>
    [Key]
    [MaxLength(50), Unicode(false)]
    public string Id { get; protected set; } = Guid.NewGuid().ToString().ToLower();

    /// <summary>
    /// 所属的自然人Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string PersonId { get; protected internal set; } = default!;

    /// <summary>
    /// 与此实名认证有关的个人名称信息。
    /// </summary>
    public PersonNameInfo PersonName { get; protected set; } = default!;

    /// <summary>
    /// 认证通过的时间。
    /// </summary>
    public DateTimeOffset ValidatedAt { get; protected set; }

    /// <summary>
    /// 认证执行者。
    /// </summary>
    [MaxLength(30)]
    public string ValidatedBy { get; protected set; } = default!;

    /// <summary>
    /// 有效期限。
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; protected set; }

    /// <summary>
    /// 备注和其他信息
    /// </summary>
    [MaxLength(200)]
    public string? Remark { get; set; }

    /// <summary>
    /// 是否已经应用。
    /// </summary>
    public bool Applied { get; protected internal set; }

    /// <summary>
    /// 将实名认证信息应用到自然人。
    /// </summary>
    /// <param name="person"></param>
    public virtual void ApplyToPerson(NaturalPerson person)
    {
        person.PersonName = this.PersonName;
    }
}