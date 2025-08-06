namespace AdminWebApp.Domain.Security;

/// <summary>
/// Role.
/// </summary>
public class Role
{
    /// <summary>
    /// Role Name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Display Name.
    /// </summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// Description.
    /// </summary>
    public string? Description { get; set; }
}