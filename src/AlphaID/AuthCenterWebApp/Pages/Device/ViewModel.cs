namespace AuthCenterWebApp.Pages.Device;

public class ViewModel
{
    public string ClientName { get; set; } = default!;
    public string ClientUrl { get; set; } = default!;
    public string ClientLogoUrl { get; set; } = default!;
    public bool AllowRememberConsent { get; set; }

    public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
    public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
}

public class ScopeViewModel
{
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool Emphasize { get; set; }
    public bool Required { get; set; }
    public bool Checked { get; set; }
}