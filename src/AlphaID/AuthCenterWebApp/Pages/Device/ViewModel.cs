namespace AuthCenterWebApp.Pages.Device;

public class ViewModel
{
    public string ClientName { get; init; } = default!;
    public string? ClientUrl { get; init; }
    public string? ClientLogoUrl { get; init; }
    public bool AllowRememberConsent { get; init; }

    public IEnumerable<ScopeViewModel> IdentityScopes { get; init; } = default!;
    public IEnumerable<ScopeViewModel> ApiScopes { get; set; } = default!;
}

public class ScopeViewModel
{
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Description { get; set; }
    public bool Emphasize { get; set; }
    public bool Required { get; set; }
    public bool Checked { get; set; }
}