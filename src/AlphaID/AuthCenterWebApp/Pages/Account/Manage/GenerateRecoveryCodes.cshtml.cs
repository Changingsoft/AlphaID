#nullable disable

using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account.Manage;

public class GenerateRecoveryCodesModel : PageModel
{
    private readonly NaturalPersonManager _userManager;
    private readonly ILogger<GenerateRecoveryCodesModel> _logger;

    public GenerateRecoveryCodesModel(
        NaturalPersonManager userManager,
        ILogger<GenerateRecoveryCodesModel> logger)
    {
        this._userManager = userManager;
        this._logger = logger;
    }

    [TempData]
    public string[] RecoveryCodes { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        var isTwoFactorEnabled = await this._userManager.GetTwoFactorEnabledAsync(user);
        return !isTwoFactorEnabled
            ? throw new InvalidOperationException($"Cannot generate recovery codes for user because they do not have 2FA enabled.")
            : (IActionResult)this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        var isTwoFactorEnabled = await this._userManager.GetTwoFactorEnabledAsync(user);
        var userId = await this._userManager.GetUserIdAsync(user);
        if (!isTwoFactorEnabled)
        {
            throw new InvalidOperationException($"Cannot generate recovery codes for user as they do not have 2FA enabled.");
        }

        var recoveryCodes = await this._userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        this.RecoveryCodes = recoveryCodes.ToArray();

        this._logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
        this.StatusMessage = "You have generated new recovery codes.";
        return this.RedirectToPage("./ShowRecoveryCodes");
    }
}
