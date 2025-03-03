using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// 组织的曾用名。
/// </summary>
[Owned]
[Table("OrganizationUsedName")]
public class OrganizationUsedName
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    public int Id { get; protected set; }

    /// <summary>
    /// 名称。
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 弃用日期。
    /// </summary>
    public DateOnly DeprecateTime { get; set; }
}