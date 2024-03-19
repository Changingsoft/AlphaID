#nullable disable

using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class TwoFactorAuthenticationModel(
    NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager, ILogger<TwoFactorAuthenticationModel> logger) : PageModel
{
    public bool HasAuthenticator { get; set; }

    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2FaEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }

        this.HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null;
        this.Is2FaEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        this.IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
        this.RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }

        await signInManager.ForgetTwoFactorClientAsync();
        this.StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
        return this.RedirectToPage();
    }
}
