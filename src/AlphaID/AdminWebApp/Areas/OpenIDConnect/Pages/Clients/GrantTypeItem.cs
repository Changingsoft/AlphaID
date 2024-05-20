namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

/// <summary>
/// </summary>
/// <param name="Name"></param>
/// <param name="DisplayName"></param>
/// <param name="Description"></param>
public record GrantTypeItem(string Name, string DisplayName, string? Description = null);