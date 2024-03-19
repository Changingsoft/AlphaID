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
public class FindPasswordByMobileModel(NaturalPersonManager userManager, IVerificationCodeService verificationCodeService) : PageModel
{
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

        var person = await userManager.FindByMobileAsync(normalPhoneNumber, this.HttpContext.RequestAborted);
        if (person is not { PhoneNumberConfirmed: true })
        {
            //不执行操作
            return this.RedirectToPage("ResetPasswordMobile", new { code, phone = this.Mobile });
        }

        code = await userManager.GeneratePasswordResetTokenAsync(person);

        await verificationCodeService.SendAsync(normalPhoneNumber);
        return this.RedirectToPage("ResetPasswordMobile", new { code, phone = this.Mobile });
    }
}
