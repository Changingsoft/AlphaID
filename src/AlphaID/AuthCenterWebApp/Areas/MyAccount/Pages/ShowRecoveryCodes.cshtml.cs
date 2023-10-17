#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.MyAccount.Pages;

public class ShowRecoveryCodesModel : PageModel
{
    [TempData]
    public string[] RecoveryCodes { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public IActionResult OnGet()
    {
        return this.RecoveryCodes == null || this.RecoveryCodes.Length == 0 ? this.RedirectToPage("./TwoFactorAuthentication") : this.Page();
    }
}
