// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace AuthCenterWebApp.Pages.Consent;

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
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Description { get; set; }
    public bool Emphasize { get; set; }
    public bool Required { get; set; }
    public bool Checked { get; set; }
    public IEnumerable<ResourceViewModel> Resources { get; set; } = default!;
}

public class ResourceViewModel
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; init; } = default!;
}