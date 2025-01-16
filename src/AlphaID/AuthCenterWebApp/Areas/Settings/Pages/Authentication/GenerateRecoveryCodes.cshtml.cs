using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class GenerateRecoveryCodesModel(
    ApplicationUserManager userManager,
    ILogger<GenerateRecoveryCodesModel> logger) : PageModel
{
    [TempData]
    public string[]? RecoveryCodes { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        bool isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        return !isTwoFactorEnabled
            ? throw new InvalidOperationException(
                "Cannot generate recovery codes for user because they do not have 2FA enabled.")
            : (IActionResult)Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        bool isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        string userId = await userManager.GetUserIdAsync(user);
        if (!isTwoFactorEnabled)
            throw new InvalidOperationException(
                "Cannot generate recovery codes for user as they do not have 2FA enabled.");

        IEnumerable<string>? recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        RecoveryCodes = recoveryCodes?.ToArray();

        logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
        StatusMessage = "You have generated new recovery codes.";
        return RedirectToPage("./ShowRecoveryCodes");
    }
}