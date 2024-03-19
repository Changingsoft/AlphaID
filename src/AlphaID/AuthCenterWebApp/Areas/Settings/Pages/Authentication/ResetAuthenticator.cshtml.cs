#nullable disable

using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class ResetAuthenticatorModel(
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager,
    ILogger<ResetAuthenticatorModel> logger) : PageModel
{
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        return user == null ? this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);
        _ = await userManager.GetUserIdAsync(user);
        logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

        await signInManager.RefreshSignInAsync(user);
        this.StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

        return this.RedirectToPage("./EnableAuthenticator");
    }
}
