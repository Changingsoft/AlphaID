using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// 组织的人员。
/// </summary>
[Table("OrganizationMember")]
[PrimaryKey(nameof(PersonId), nameof(OrganizationId))]
public class OrganizationMember
{
    /// <summary>
    /// </summary>
    protected OrganizationMember()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="userId"></param>
    /// <param name="visibility"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrganizationMember(Organization organization, string userId, MembershipVisibility visibility)
    {
        OrganizationId = organization.Id;
        Organization = organization ?? throw new ArgumentNullException(nameof(organization));
        PersonId = userId;
        Visibility = visibility;
    }

    /// <summary>
    /// Organization Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string OrganizationId { get; protected set; } = null!;

    /// <summary>
    /// Person Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string PersonId { get; protected set; } = null!;

    /// <summary>
    /// Organization.
    /// </summary>
    [ForeignKey(nameof(OrganizationId))]
    [Obsolete("作为拥有类型不再被引用。")]
    public virtual Organization Organization { get; protected set; } = null!;

    /// <summary>
    /// 姓名。
    /// </summary>
    [MaxLength(50)]
    public string PersonName { get; set; } = null!;

    /// <summary>
    /// 用户名。
    /// </summary>
    [MaxLength(50)]
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 部门。
    /// </summary>
    [MaxLength(50)]
    public string? Department { get; set; } = null!;

    /// <summary>
    /// 职务。
    /// </summary>
    [MaxLength(50)]
    public string? Title { get; set; } = null!;

    /// <summary>
    /// 备注。
    /// </summary>
    [MaxLength(50)]
    public string? Remark { get; set; } = null!;

    /// <summary>
    /// Is Owner of the organization.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Membership visibility.
    /// </summary>
    public virtual MembershipVisibility Visibility { get; set; } = MembershipVisibility.Private;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Organization.Name}|{PersonId}|{(IsOwner ? "Owner" : "")}|{Visibility}";
    }
}