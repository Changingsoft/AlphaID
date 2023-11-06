#nullable disable

using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class TwoFactorAuthenticationModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly SignInManager<NaturalPerson> signInManager;
    private readonly ILogger<TwoFactorAuthenticationModel> logger;

    public TwoFactorAuthenticationModel(
        NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager, ILogger<TwoFactorAuthenticationModel> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
    }

    public bool HasAuthenticator { get; set; }

    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2FaEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        this.HasAuthenticator = await this.userManager.GetAuthenticatorKeyAsync(user) != null;
        this.Is2FaEnabled = await this.userManager.GetTwoFactorEnabledAsync(user);
        this.IsMachineRemembered = await this.signInManager.IsTwoFactorClientRememberedAsync(user);
        this.RecoveryCodesLeft = await this.userManager.CountRecoveryCodesAsync(user);

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        await this.signInManager.ForgetTwoFactorClientAsync();
        this.StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
        return this.RedirectToPage();
    }
}
