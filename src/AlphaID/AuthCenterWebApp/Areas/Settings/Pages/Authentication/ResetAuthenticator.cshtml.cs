#nullable disable

using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class ResetAuthenticatorModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly SignInManager<NaturalPerson> signInManager;
    private readonly ILogger<ResetAuthenticatorModel> logger;

    public ResetAuthenticatorModel(
        NaturalPersonManager userManager,
        SignInManager<NaturalPerson> signInManager,
        ILogger<ResetAuthenticatorModel> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
    }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        return user == null ? this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        await this.userManager.SetTwoFactorEnabledAsync(user, false);
        await this.userManager.ResetAuthenticatorKeyAsync(user);
        _ = await this.userManager.GetUserIdAsync(user);
        this.logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

        await this.signInManager.RefreshSignInAsync(user);
        this.StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

        return this.RedirectToPage("./EnableAuthenticator");
    }
}
