#nullable disable

using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class Disable2FaModel(
    NaturalPersonManager userManager,
    ILogger<Disable2FaModel> logger) : PageModel
{
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        return user == null
            ? this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.")
            : !await userManager.GetTwoFactorEnabledAsync(user)
            ? throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.")
            : (IActionResult)this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }

        var disable2FaResult = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2FaResult.Succeeded)
        {
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");
        }

        logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userManager.GetUserId(this.User));
        this.StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
        return this.RedirectToPage("./TwoFactorAuthentication");
    }
}
