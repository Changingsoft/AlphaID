using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects;

/// <summary>
/// 组织的信息管理员
/// </summary>
[Obsolete("组织的管理员将来合并到组织的成员管理中，不再单独设置管理员。")]
[Table("OrganizationAdministrator")]
[PrimaryKey(nameof(OrganizationId), nameof(PersonId))]
public class OrganizationAdministrator
{
    /// <summary>
    /// 组织Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string OrganizationId { get; set; } = default!;

    /// <summary>
    /// 管理者Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string PersonId { get; set; } = default!;

    /// <summary>
    /// 管理人名称。
    /// </summary>
    [MaxLength(20)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 指示该管理者是否是该组织的创建者
    /// </summary>
    public bool IsOrganizationCreator { get; protected internal set; }

    /// <summary>
    /// GenericOrganization.
    /// </summary>
    [ForeignKey(nameof(OrganizationId))]
    public virtual GenericOrganization Organization { get; protected set; } = default!;
}
