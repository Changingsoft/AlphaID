// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace AuthCenterWebApp.Pages.Account.Logout;

public class LoggedOutViewModel
{
    public string PostLogoutRedirectUri { get; set; }
    public string ClientName { get; set; } = default!;
    public string SignOutIframeUrl { get; set; }
    public bool AutomaticRedirectAfterSignOut { get; set; }
}