using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Pages.Account;

[AllowAnonymous]
public class SignOutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        string? signedOutUri = Url.Page("/Account/SignedOut");
        await HttpContext.SignOutAsync();
        return SignOut(new AuthenticationProperties { RedirectUri = signedOutUri },
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}