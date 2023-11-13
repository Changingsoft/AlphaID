using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects;

/// <summary>
/// 
/// </summary>
[Table("OrganizationIdentifier")]
[PrimaryKey(nameof(Value), nameof(Type))]
public class OrganizationIdentifier
{
    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string OrganizationId { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(OrganizationId))]
    public virtual GenericOrganization Organization { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public OrganizationIdentifierType Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string Value { get; set; } = default!;
}