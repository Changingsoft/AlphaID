#nullable disable

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class LoginWithRecoveryCodeModel : PageModel
{
    private readonly SignInManager<NaturalPerson> _signInManager;
    private readonly NaturalPersonManager _userManager;
    private readonly ILogger<LoginWithRecoveryCodeModel> _logger;
    private readonly IIdentityServerInteractionService interactionService;
    private readonly IEventService eventService;

    public LoginWithRecoveryCodeModel(
        SignInManager<NaturalPerson> signInManager,
        NaturalPersonManager userManager,
        ILogger<LoginWithRecoveryCodeModel> logger,
        IIdentityServerInteractionService interactionService,
        IEventService eventService)
    {
        this._signInManager = signInManager;
        this._userManager = userManager;
        this._logger = logger;
        this.interactionService = interactionService;
        this.eventService = eventService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [BindProperty]
        [Required(ErrorMessage = "{0}是必需的")]
        [DataType(DataType.Text)]
        [Display(Name = "恢复代码")]
        public string RecoveryCode { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        _ = await this._signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        this.ReturnUrl = returnUrl;

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = await this._signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        var recoveryCode = this.Input.RecoveryCode.Replace(" ", string.Empty);

        var result = await this._signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        _ = await this._userManager.GetUserIdAsync(user);

        var context = await this.interactionService.GetAuthorizationContextAsync(returnUrl);

        if (result.Succeeded)
        {
            this._logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);

            await this.eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(returnUrl);
                }

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return this.Redirect(returnUrl);
            }

            // request for a local page
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else if (string.IsNullOrEmpty(returnUrl))
            {
                return this.Redirect("~/");
            }
            else
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
        }
        if (result.IsLockedOut)
        {
            this._logger.LogWarning("User account locked out.");
            return this.RedirectToPage("./Lockout");
        }
        else
        {
            this._logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
            this.ModelState.AddModelError(string.Empty, "恢复代码无效。");
            return this.Page();
        }
    }
}
