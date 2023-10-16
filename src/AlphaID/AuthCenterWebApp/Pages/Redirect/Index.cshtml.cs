using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Redirect;

[AllowAnonymous]
public class IndexModel : PageModel
{
    public string RedirectUri { get; set; } = default!;

    public IActionResult OnGet(string redirectUri)
    {
        if (!this.Url.IsLocalUrl(redirectUri))
        {
            return this.RedirectToPage("/Home/Error/LoginModel");
        }

        this.RedirectUri = redirectUri;
        return this.Page();
    }
}