#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using AlphaIdPlatform;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class EnableAuthenticatorModel(
    ApplicationUserManager<ApplicationUser> userManager,
    ILogger<EnableAuthenticatorModel> logger,
    UrlEncoder urlEncoder,
    IOptions<ProductInfo> production) : PageModel
{
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    private readonly ProductInfo _production = production.Value;

    public string SharedKey { get; set; }

    public string AuthenticatorUri { get; set; }

    [TempData]
    public string[] RecoveryCodes { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        await LoadSharedKeyAndQrCodeUriAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUser user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return Page();
        }

        // Strip spaces and hyphens
        string verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        bool is2FaTokenValid = await userManager.VerifyTwoFactorTokenAsync(
            user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2FaTokenValid)
        {
            ModelState.AddModelError("Input.Code", "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return Page();
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        string userId = await userManager.GetUserIdAsync(user);
        logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        StatusMessage = "Your authenticator app has been verified.";

        if (await userManager.CountRecoveryCodesAsync(user) == 0)
        {
            IEnumerable<string> recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            RecoveryCodes = recoveryCodes.ToArray();
            return RedirectToPage("./ShowRecoveryCodes");
        }

        return RedirectToPage("./TwoFactorAuthentication");
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
    {
        // Load the authenticator key & QR code URI to display on the form
        string unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        SharedKey = FormatKey(unformattedKey);

        string email = await userManager.GetEmailAsync(user);
        AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length) result.Append(unformattedKey.AsSpan(currentPosition));

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            urlEncoder.Encode($"{_production.Name} Auth Center"), //Title
            urlEncoder.Encode(email),
            unformattedKey);
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(7, ErrorMessage = "Validate_StringLength", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
    }
}