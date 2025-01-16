using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class LoginWithRecoveryCodeModel(
    SignInManager<ApplicationUser> signInManager,
    ApplicationUserManager<ApplicationUser> userManager,
    ILogger<LoginWithRecoveryCodeModel> logger,
    IIdentityServerInteractionService interactionService,
    IEventService eventService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        _ = await signInManager.GetTwoFactorAuthenticationUserAsync() ??
            throw new InvalidOperationException("Unable to load two-factor authentication user.");
        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid) return Page();

        ApplicationUser user = await signInManager.GetTwoFactorAuthenticationUserAsync() ??
                             throw new InvalidOperationException("Unable to load two-factor authentication user.");
        string recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

        SignInResult result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        _ = await userManager.GetUserIdAsync(user);

        AuthorizationRequest? context = await interactionService.GetAuthorizationContextAsync(returnUrl);

        if (result.Succeeded)
        {
            logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);

            await eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName,
                clientId: context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(returnUrl);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(returnUrl);
            }

            // request for a local page
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            if (string.IsNullOrEmpty(returnUrl))
                return Redirect("~/");
            // user might have clicked on a malicious link - should be logged
            throw new Exception("invalid return URL");
        }

        if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }

        logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
        ModelState.AddModelError(string.Empty, "恢复代码无效。");
        return Page();
    }

    public class InputModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Validate_Required")]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery code")]
        public string RecoveryCode { get; set; } = null!;
    }
}