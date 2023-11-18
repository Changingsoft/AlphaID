using AlphaIdPlatform.Platform;
using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ResetPasswordMobileModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly IVerificationCodeService verificationCodeService;

    public ResetPasswordMobileModel(NaturalPersonManager userManager, IVerificationCodeService verificationCodeService)
    {
        this.userManager = userManager;
        this.verificationCodeService = verificationCodeService;
    }

    [BindProperty]
    public string Code { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [Display(Name = "PhoneNumber phone number")]
    public string PhoneNumber { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [Display(Name = "Verification code")]
    public string VerificationCode { get; set; } = default!;

    [BindProperty]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Validate_Required")]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = default!;

    [BindProperty]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Validate_Required")]
    [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = default!;

    public IActionResult OnGet(string code, string phone)
    {
        if (code == null)
            return this.BadRequest("A code must be supplied for password reset.");

        this.Code = code;
        this.PhoneNumber = phone;

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!MobilePhoneNumber.TryParse(this.PhoneNumber, out var phone))
        {
            this.ModelState.AddModelError(nameof(this.PhoneNumber), "移动电话号码无效");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        var normalPhoneNumber = phone.ToString();

        var person = await this.userManager.FindByMobileAsync(normalPhoneNumber, this.HttpContext.RequestAborted);
        if (person is not { PhoneNumberConfirmed: true })
        {
            return this.RedirectToPage("ResetPasswordConfirmation");
        }

        if (!await this.verificationCodeService.VerifyAsync(this.PhoneNumber, this.VerificationCode))
        {
            return this.RedirectToPage("ResetPasswordConfirmation");
        }

        var result = await this.userManager.ResetPasswordAsync(person, this.Code, this.NewPassword);
        if (result.Succeeded)
        {
            return this.RedirectToPage("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError("", error.Description);
        }
        return this.Page();
    }
}
