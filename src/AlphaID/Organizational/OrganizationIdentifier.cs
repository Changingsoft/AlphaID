
namespace Organizational;

/// <summary>
/// </summary>
[Obsolete("不再考虑使用此类型。")]
public class OrganizationIdentifier
{
    /// <summary>
    /// </summary>
    public OrganizationIdentifierType Type { get; set; }

    /// <summary>
    /// </summary>
    public string Value { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string OrganizationId { get; set; } = null!;
}