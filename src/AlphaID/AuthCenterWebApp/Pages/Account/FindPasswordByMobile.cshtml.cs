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
public class FindPasswordByMobileModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly IVerificationCodeService verificationCodeService;

    public FindPasswordByMobileModel(NaturalPersonManager userManager, IVerificationCodeService verificationCodeService)
    {
        this.userManager = userManager;
        this.verificationCodeService = verificationCodeService;
    }

    [Display(Name = "PhoneNumber phone number")]
    [Required(ErrorMessage = "Validate_Required")]
    [BindProperty]
    public string Mobile { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!MobilePhoneNumber.TryParse(this.Mobile, out var phoneNumber))
        {
            this.ModelState.AddModelError(nameof(this.Mobile), "无效的移动电话号码");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        var code = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var normalPhoneNumber = phoneNumber.ToString();

        var person = this.userManager.Users.FirstOrDefault(p => p.PhoneNumber == normalPhoneNumber);
        if (person == null || !person.PhoneNumberConfirmed)
        {
            //不执行操作
            return this.RedirectToPage("ResetPasswordMobile", new { code, phone = this.Mobile });
        }

        code = await this.userManager.GeneratePasswordResetTokenAsync(person);

        await this.verificationCodeService.SendAsync(normalPhoneNumber);
        return this.RedirectToPage("ResetPasswordMobile", new { code, phone = this.Mobile });
    }
}
