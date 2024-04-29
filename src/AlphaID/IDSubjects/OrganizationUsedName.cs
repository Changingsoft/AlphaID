using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects;

/// <summary>
///     组织的曾用名。
/// </summary>
[Table("OrganizationUsedName")]
public class OrganizationUsedName
{
    /// <summary>
    ///     ID
    /// </summary>
    [Key]
    public int Id { get; protected set; }

    /// <summary>
    ///     组织Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string OrganizationId { get; protected set; } = default!;

    /// <summary>
    ///     所属组织
    /// </summary>
    [ForeignKey(nameof(OrganizationId))]
    public virtual GenericOrganization Organization { get; protected set; } = default!;

    /// <summary>
    ///     名称。
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    /// <summary>
    ///     弃用日期。
    /// </summary>
    public DateOnly DeprecateTime { get; set; }
}