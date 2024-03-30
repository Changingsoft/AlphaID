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
public class ResetPasswordMobileModel(NaturalPersonManager userManager, IVerificationCodeService verificationCodeService) : PageModel
{
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
            return BadRequest("A code must be supplied for password reset.");

        Code = code;
        PhoneNumber = phone;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!MobilePhoneNumber.TryParse(PhoneNumber, out var phone))
        {
            ModelState.AddModelError(nameof(PhoneNumber), "移动电话号码无效");
        }

        if (!ModelState.IsValid)
            return Page();

        var normalPhoneNumber = phone.ToString();

        var person = await userManager.FindByMobileAsync(normalPhoneNumber, HttpContext.RequestAborted);
        if (person is not { PhoneNumberConfirmed: true })
        {
            return RedirectToPage("ResetPasswordConfirmation");
        }

        if (!await verificationCodeService.VerifyAsync(PhoneNumber, VerificationCode))
        {
            return RedirectToPage("ResetPasswordConfirmation");
        }

        var result = await userManager.ResetPasswordAsync(person, Code, NewPassword);
        if (result.Succeeded)
        {
            return RedirectToPage("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return Page();
    }
}
