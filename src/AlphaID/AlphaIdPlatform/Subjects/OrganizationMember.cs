using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// 组织的人员。
/// </summary>
[Table("OrganizationMember")]
[PrimaryKey(nameof(OrganizationId), nameof(PersonId))]
[Index(nameof(PersonId))]
[Owned]
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
    /// <param name="userId"></param>
    /// <param name="visibility"></param>
    /// 
    /// <exception cref="ArgumentNullException"></exception>
    public OrganizationMember(string userId, MembershipVisibility visibility)
    {
        PersonId = userId;
        Visibility = visibility;
    }

    /// <summary>
    /// Person Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string PersonId { get; protected set; } = null!;

    /// <summary>
    /// Gets the unique identifier for the organization.
    /// </summary>
    public string OrganizationId { get; protected set; } = null!;

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
        return $"{PersonId}|{(IsOwner ? "Owner" : "")}|{Visibility}";
    }
}