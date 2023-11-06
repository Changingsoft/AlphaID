namespace AdminWebApp.Domain.Security;

/// <summary>
/// Role.
/// </summary>
public class Role
{
    /// <summary>
    /// Role Name.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Display Name.
    /// </summary>
    public string DisplayName { get; init; } = default!;
}
