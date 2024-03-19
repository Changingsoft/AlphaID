using AlphaIdPlatform.Identity;
using Duende.IdentityServer.Services;
using IdSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ChangePasswordModel(NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager, IIdentityServerInteractionService interaction) : PageModel
{
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = default!;


    public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
    {
        //Ensure user must change password
        var result = await this.HttpContext.AuthenticateAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        if (result.Principal == null)
        {
            throw new InvalidOperationException("Unable to load must change password authentication user.");
        }
        string personId = result.Principal.FindFirstValue(ClaimTypes.Name) ?? throw new InvalidOperationException("Unable to load must change password authentication user.");
        _ = await userManager.FindByIdAsync(personId) ?? throw new InvalidOperationException("Unable to load must change password authentication user.");
        this.RememberMe = rememberMe;
        this.ReturnUrl = returnUrl;

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
    {
        //Ensure user must change password
        var authMustChangePasswordResult = await this.HttpContext.AuthenticateAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        if (authMustChangePasswordResult.Principal == null)
        {
            throw new InvalidOperationException("Unable to load must change password authentication user.");
        }
        string personId = authMustChangePasswordResult.Principal.FindFirstValue(ClaimTypes.Name) ?? throw new InvalidOperationException("Unable to load must change password authentication user.");
        var person = await userManager.FindByIdAsync(personId) ?? throw new InvalidOperationException("Unable to load must change password authentication user.");
        var identityResult = await userManager.ChangePasswordAsync(person, this.Input.OldPassword, this.Input.NewPassword);
        if (identityResult.Succeeded)
        {
            //Sign out MustChangePasswordScheme
            await this.HttpContext.SignOutAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme);

            //Signin user without password.
            await signInManager.SignInAsync(person, rememberMe);

            var context = await interaction.GetAuthorizationContextAsync(returnUrl);

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(returnUrl!);
                }

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return this.Redirect(returnUrl!);
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

        this.ModelState.AddModelError("", "�������������Ч��");
        foreach (var error in identityResult.Errors)
        {
            this.ModelState.AddModelError("", error.Description);
        }
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "Current password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Validate_Required")]
        public string OldPassword { get; set; } = default!;

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Validate_Required")]
        public string NewPassword { get; set; } = default!;

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
        [Required(ErrorMessage = "Validate_Required")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
