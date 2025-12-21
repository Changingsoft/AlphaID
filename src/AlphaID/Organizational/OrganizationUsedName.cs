
namespace Organizational;

/// <summary>
/// 组织的曾用名。
/// </summary>
public class OrganizationUsedName
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// 名称。
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 弃用日期。
    /// </summary>
    public DateOnly DeprecateTime { get; set; }
}