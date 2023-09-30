using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Challenge : PageModel
{
    private readonly IIdentityServerInteractionService _interactionService;
    private readonly OpenIdConnectOptions _openIdConnectOptions;

    public Challenge(IIdentityServerInteractionService interactionService, IOptionsFactory<OpenIdConnectOptions> openIdConnectOptions)
    {
        this._interactionService = interactionService;
        this._openIdConnectOptions = openIdConnectOptions.Create("netauth-8088.changingsoft.com");
    }

    public IActionResult OnGet(string scheme, string schemeDisplayName, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (this.Url.IsLocalUrl(returnUrl) == false && this._interactionService.IsValidReturnUrl(returnUrl) == false)
        {
            throw new Exception("invalid return URL");
        }

        // start challenge and roundtrip the return URL and scheme 
        var props = new AuthenticationProperties
        {
            RedirectUri = this.Url.Page("/externallogin/callback"),

            Items =
            {
                { "returnUrl", returnUrl },
                { "schemeDisplayName", schemeDisplayName },
                { "scheme", scheme },
            }
        };
        return this.Challenge(props, scheme);
    }
}