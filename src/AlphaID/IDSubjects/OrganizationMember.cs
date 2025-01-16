using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects;

/// <summary>
///     组织的人员。
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
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="person"></param>
    public OrganizationMember(Organization organization, ApplicationUser person)
    {
        OrganizationId = organization.Id;
        Organization = organization ?? throw new ArgumentNullException(nameof(organization));
        PersonId = person.Id;
        Person = person ?? throw new ArgumentNullException(nameof(person));
    }

    /// <summary>
    ///     Organization Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string OrganizationId { get; protected set; } = null!;

    /// <summary>
    ///     Person Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string PersonId { get; protected set; } = null!;

    /// <summary>
    ///     Organization.
    /// </summary>
    [ForeignKey(nameof(OrganizationId))]
    public Organization Organization { get; protected set; } = null!;

    /// <summary>
    ///     Person.
    /// </summary>
    [ForeignKey(nameof(PersonId))]
    public ApplicationUser Person { get; protected set; } = null!;

    /// <summary>
    ///     部门。
    /// </summary>
    [MaxLength(50)]
    public string? Department { get; set; } = null!;

    /// <summary>
    ///     职务。
    /// </summary>
    [MaxLength(50)]
    public string? Title { get; set; } = null!;

    /// <summary>
    ///     备注。
    /// </summary>
    [MaxLength(50)]
    public string? Remark { get; set; } = null!;

    /// <summary>
    ///     Is Owner of the organization.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    ///     Membership visibility.
    /// </summary>
    public virtual MembershipVisibility Visibility { get; set; } = MembershipVisibility.Private;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Organization.Name}|{Person.UserName}|{(IsOwner ? "Owner" : "")}|{Visibility}";
    }
}