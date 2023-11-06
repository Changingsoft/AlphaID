#nullable disable

using AlphaIDPlatform;
using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class EnableAuthenticatorModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly ILogger<EnableAuthenticatorModel> logger;
    private readonly UrlEncoder urlEncoder;
    private readonly ProductInfo production;

    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    public EnableAuthenticatorModel(
        NaturalPersonManager userManager,
        ILogger<EnableAuthenticatorModel> logger,
        UrlEncoder urlEncoder,
        IOptions<ProductInfo> production)
    {
        this.userManager = userManager;
        this.logger = logger;
        this.urlEncoder = urlEncoder;
        this.production = production.Value;
    }

    public string SharedKey { get; set; }

    public string AuthenticatorUri { get; set; }

    [TempData]
    public string[] RecoveryCodes { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(7, ErrorMessage = "Validate_StringLength", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; init; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        await this.LoadSharedKeyAndQrCodeUriAsync(user);

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        if (!this.ModelState.IsValid)
        {
            await this.LoadSharedKeyAndQrCodeUriAsync(user);
            return this.Page();
        }

        // Strip spaces and hyphens
        var verificationCode = this.Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2FaTokenValid = await this.userManager.VerifyTwoFactorTokenAsync(
            user, this.userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2FaTokenValid)
        {
            this.ModelState.AddModelError("Input.Code", "Verification code is invalid.");
            await this.LoadSharedKeyAndQrCodeUriAsync(user);
            return this.Page();
        }

        await this.userManager.SetTwoFactorEnabledAsync(user, true);
        var userId = await this.userManager.GetUserIdAsync(user);
        this.logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        this.StatusMessage = "Your authenticator app has been verified.";

        if (await this.userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await this.userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            this.RecoveryCodes = recoveryCodes.ToArray();
            return this.RedirectToPage("./ShowRecoveryCodes");
        }
        else
        {
            return this.RedirectToPage("./TwoFactorAuthentication");
        }
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(NaturalPerson user)
    {
        // Load the authenticator key & QR code URI to display on the form
        var unformattedKey = await this.userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await this.userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await this.userManager.GetAuthenticatorKeyAsync(user);
        }

        this.SharedKey = this.FormatKey(unformattedKey);

        var email = await this.userManager.GetEmailAsync(user);
        this.AuthenticatorUri = this.GenerateQrCodeUri(email, unformattedKey);
    }

    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            this.urlEncoder.Encode($"{this.production.Name} Auth Center"), //Title
            this.urlEncoder.Encode(email),
            unformattedKey);
    }
}
