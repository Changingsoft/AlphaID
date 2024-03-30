using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Challenge(IIdentityServerInteractionService interactionService) : PageModel
{
    public IActionResult OnGet(string scheme, string schemeDisplayName, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (Url.IsLocalUrl(returnUrl) == false && interactionService.IsValidReturnUrl(returnUrl) == false)
        {
            throw new Exception("invalid return URL");
        }

        // start challenge and roundtrip the return URL and scheme 
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Page("/ExternalLogin/Callback"),

            Items =
            {
                { "returnUrl", returnUrl },
                { "schemeDisplayName", schemeDisplayName },
                { "scheme", scheme },
            }
        };
        return Challenge(props, scheme);
    }
}