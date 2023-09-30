using AlphaIDPlatform.Platform;
using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ResetPasswordMobileModel : PageModel
{
    private readonly NaturalPersonManager _userManager;
    private readonly IVerificationCodeService _verificationCodeService;

    public ResetPasswordMobileModel(NaturalPersonManager userManager, IVerificationCodeService verificationCodeService)
    {
        this._userManager = userManager;
        this._verificationCodeService = verificationCodeService;
    }

    [BindProperty]
    public string Code { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "{0}是必需的")]
    [Display(Name = "移动电话号码")]
    public string PhoneNumber { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "{0}是必需的")]
    [Display(Name = "短信验证码")]
    public string VerificationCode { get; set; }

    [BindProperty]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "{0}是必需的")]
    [Display(Name = "新密码")]
    public string NewPassword { get; set; }

    [BindProperty]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "{0}是必需的")]
    [Compare(nameof(NewPassword))]
    [Display(Name = "确认密码")]
    public string ConfirmPassword { get; set; }

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

        var person = this._userManager.Users.FirstOrDefault(p => p.Mobile == normalPhoneNumber);
        if (person == null || !person.PhoneNumberConfirmed)
        {
            return this.RedirectToPage("ResetPasswordConfirmation");
        }

        if (!await this._verificationCodeService.VerifyAsync(this.PhoneNumber, this.VerificationCode))
        {
            return this.RedirectToPage("ResetPasswordConfirmation");
        }

        var result = await this._userManager.ResetPasswordAsync(person, this.Code, this.NewPassword);
        if (result.Succeeded)
        {
            person.PasswordLastSet = DateTime.Now;
            await this._userManager.UpdateAsync(person);
            return this.RedirectToPage("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError("", error.Description);
        }
        return this.Page();
    }
}
