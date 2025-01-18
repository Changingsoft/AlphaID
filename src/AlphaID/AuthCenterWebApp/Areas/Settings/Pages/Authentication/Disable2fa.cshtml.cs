#nullable disable

using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class Disable2FaModel(
    UserManager<NaturalPerson> userManager,
    ILogger<Disable2FaModel> logger) : PageModel
{
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson user = await userManager.GetUserAsync(User);
        return user == null
            ? NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.")
            : !await userManager.GetTwoFactorEnabledAsync(user)
                ? throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.")
                : (IActionResult)Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        IdentityResult disable2FaResult = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2FaResult.Succeeded)
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");

        logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userManager.GetUserId(User));
        StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
        return RedirectToPage("./TwoFactorAuthentication");
    }
}