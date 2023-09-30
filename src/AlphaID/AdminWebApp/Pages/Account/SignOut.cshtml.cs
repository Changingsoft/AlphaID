using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Pages.Account;

[AllowAnonymous()]
public class SignOutModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        var signedOutUri = this.Url.Page("/Account/SignedOut");
        await this.HttpContext.SignOutAsync();
        return this.SignOut(new AuthenticationProperties { RedirectUri = signedOutUri }, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
