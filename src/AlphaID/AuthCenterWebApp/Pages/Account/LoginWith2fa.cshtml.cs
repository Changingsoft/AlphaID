#nullable disable

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

[SecurityHeaders]
[AllowAnonymous]
public class LoginWith2FaModel(
    SignInManager<ApplicationUser> signInManager,
    ApplicationUserManager<ApplicationUser> userManager,
    ILogger<LoginWith2FaModel> logger,
    IIdentityServerInteractionService interactionService,
    IEventService eventService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        _ = await signInManager.GetTwoFactorAuthenticationUserAsync() ??
            throw new InvalidOperationException("Unable to load two-factor authentication user.");
        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
    {
        if (!ModelState.IsValid) return Page();

        returnUrl ??= Url.Content("~/");
        AuthorizationRequest context = await interactionService.GetAuthorizationContextAsync(returnUrl);

        ApplicationUser user = await signInManager.GetTwoFactorAuthenticationUserAsync() ??
                             throw new InvalidOperationException("Unable to load two-factor authentication user.");
        string authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        SignInResult result =
            await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);
        _ = await userManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
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
            logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
            return RedirectToPage("./Lockout");
        }

        logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
        ModelState.AddModelError(string.Empty, "身份验证器代码无效。");
        return Page();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(7, ErrorMessage = "Validate_StringLength", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Two-factor code")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Remember this device")]
        public bool RememberMachine { get; set; }
    }
}