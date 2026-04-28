using AlphaIdPlatform.Identity;
using Duende.IdentityServer.Models;
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

/// <summary>
/// 强制更改密码。此页用于用户在通过身份验证前，必须强制要求修改密码的场景。
/// 用户在登录时，如果满足强制更改密码的条件（例如密码过期），将被重定向到此页，要求他们输入当前密码和新密码以完成密码更改过程。
/// 一旦成功更改密码，用户将被重定向回原始请求的页面或主页。
/// </summary>
/// <param name="naturalPersonService"></param>
/// <param name="userManager"></param>
/// <param name="signInManager"></param>
/// <param name="interaction"></param>
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