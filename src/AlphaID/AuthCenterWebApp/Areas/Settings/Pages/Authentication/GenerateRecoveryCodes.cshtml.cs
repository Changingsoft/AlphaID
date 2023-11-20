#nullable disable

using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class GenerateRecoveryCodesModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly ILogger<GenerateRecoveryCodesModel> logger;

    public GenerateRecoveryCodesModel(
        NaturalPersonManager userManager,
        ILogger<GenerateRecoveryCodesModel> logger)
    {
        this.userManager = userManager;
        this.logger = logger;
    }

    [TempData]
    public string[] RecoveryCodes { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        var isTwoFactorEnabled = await this.userManager.GetTwoFactorEnabledAsync(user);
        return !isTwoFactorEnabled
            ? throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.")
            : (IActionResult)this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        var isTwoFactorEnabled = await this.userManager.GetTwoFactorEnabledAsync(user);
        var userId = await this.userManager.GetUserIdAsync(user);
        if (!isTwoFactorEnabled)
        {
            throw new InvalidOperationException("Cannot generate recovery codes for user as they do not have 2FA enabled.");
        }

        var recoveryCodes = await this.userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        this.RecoveryCodes = recoveryCodes.ToArray();

        this.logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
        this.StatusMessage = "You have generated new recovery codes.";
        return this.RedirectToPage("./ShowRecoveryCodes");
    }
}
