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
    private readonly NaturalPersonManager _userManager;
    private readonly ILogger<EnableAuthenticatorModel> _logger;
    private readonly UrlEncoder _urlEncoder;
    private readonly ProductInfo production;

    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    public EnableAuthenticatorModel(
        NaturalPersonManager userManager,
        ILogger<EnableAuthenticatorModel> logger,
        UrlEncoder urlEncoder,
        IOptions<ProductInfo> production)
    {
        this._userManager = userManager;
        this._logger = logger;
        this._urlEncoder = urlEncoder;
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
        public string Code { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        await this.LoadSharedKeyAndQrCodeUriAsync(user);

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        if (!this.ModelState.IsValid)
        {
            await this.LoadSharedKeyAndQrCodeUriAsync(user);
            return this.Page();
        }

        // Strip spaces and hyphens
        var verificationCode = this.Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await this._userManager.VerifyTwoFactorTokenAsync(
            user, this._userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            this.ModelState.AddModelError("Input.Code", "Verification code is invalid.");
            await this.LoadSharedKeyAndQrCodeUriAsync(user);
            return this.Page();
        }

        await this._userManager.SetTwoFactorEnabledAsync(user, true);
        var userId = await this._userManager.GetUserIdAsync(user);
        this._logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        this.StatusMessage = "Your authenticator app has been verified.";

        if (await this._userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await this._userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
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
        var unformattedKey = await this._userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await this._userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await this._userManager.GetAuthenticatorKeyAsync(user);
        }

        this.SharedKey = this.FormatKey(unformattedKey);

        var email = await this._userManager.GetEmailAsync(user);
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
            this._urlEncoder.Encode($"{this.production.Name} Auth Center"), //Title
            this._urlEncoder.Encode(email),
            unformattedKey);
    }
}
