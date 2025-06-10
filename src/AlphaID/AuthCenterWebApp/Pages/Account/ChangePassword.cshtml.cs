using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AlphaIdPlatform.Identity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ChangePasswordModel(
    NaturalPersonService naturalPersonService,
    UserManager<NaturalPerson> userManager,
    SignInManager<NaturalPerson> signInManager,
    IIdentityServerInteractionService interaction) : PageModel
{
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
    {
        //Ensure user must change password
        AuthenticateResult result =
            await HttpContext.AuthenticateAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        if (result.Principal == null)
            throw new InvalidOperationException("Unable to load must change password authentication user.");

        string personId = result.Principal.FindFirstValue(ClaimTypes.Name) ??
                          throw new InvalidOperationException(
                              "Unable to load must change password authentication user.");
        _ = await userManager.FindByIdAsync(personId) ??
            throw new InvalidOperationException("Unable to load must change password authentication user.");
        RememberMe = rememberMe;
        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
    {
        //Ensure user must change password
        AuthenticateResult authMustChangePasswordResult =
            await HttpContext.AuthenticateAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        if (authMustChangePasswordResult.Principal == null)
            throw new InvalidOperationException("Unable to load must change password authentication user.");
        string personId = authMustChangePasswordResult.Principal.FindFirstValue(ClaimTypes.Name) ??
                          throw new InvalidOperationException(
                              "Unable to load must change password authentication user.");
        NaturalPerson person = await userManager.FindByIdAsync(personId) ??
                               throw new InvalidOperationException(
                                   "Unable to load must change password authentication user.");
        Result = await naturalPersonService.ChangePasswordAsync(person, Input.OldPassword, Input.NewPassword);

        if (!Result.Succeeded) 
            return Page();

        //Sign out MustChangePasswordScheme
        await HttpContext.SignOutAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme);

        //Signin user without password.
        await signInManager.SignInAsync(person, rememberMe);

        AuthorizationRequest? context = await interaction.GetAuthorizationContextAsync(returnUrl);

        if (context != null)
        {
            if (context.IsNativeClient())
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl!);

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(returnUrl!);
        }

        // request for a local page
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        if (string.IsNullOrEmpty(returnUrl))
            return Redirect("~/");
        // user might have clicked on a malicious link - should be logged
        throw new Exception("invalid return URL");

    }

    public class InputModel
    {
        [Display(Name = "Current password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Validate_Required")]
        public string OldPassword { get; set; } = null!;

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Validate_Required")]
        public string NewPassword { get; set; } = null!;

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
        [Required(ErrorMessage = "Validate_Required")]
        public string ConfirmPassword { get; set; } = null!;
    }
}