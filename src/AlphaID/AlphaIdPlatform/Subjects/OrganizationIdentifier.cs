using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// </summary>
[Owned]
[Table("OrganizationIdentifier")]
[PrimaryKey(nameof(Value), nameof(OrganizationId), nameof(Type))]
public class OrganizationIdentifier
{
    /// <summary>
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public OrganizationIdentifierType Type { get; set; }

    /// <summary>
    /// </summary>
    [MaxLength(30)]
    public string Value { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string OrganizationId { get; set; } = null!;
}