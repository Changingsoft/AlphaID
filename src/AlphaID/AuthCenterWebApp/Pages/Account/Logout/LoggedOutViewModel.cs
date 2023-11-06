// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace AuthCenterWebApp.Pages.Account.Logout;

public class LoggedOutViewModel
{
    public string? PostLogoutRedirectUri { get; init; }
    public string? ClientName { get; init; } = default!;
    public string? SignOutIframeUrl { get; init; }
    public bool AutomaticRedirectAfterSignOut { get; init; }
}