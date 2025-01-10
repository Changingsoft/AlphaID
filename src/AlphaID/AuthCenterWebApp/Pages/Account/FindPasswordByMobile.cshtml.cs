using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Platform;
using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class FindPasswordByMobileModel(
    NaturalPersonManager userManager,
    IVerificationCodeService verificationCodeService) : PageModel
{
    [Display(Name = "PhoneNumber phone number")]
    [Required(ErrorMessage = "Validate_Required")]
    [BindProperty]
    public string Mobile { get; set; } = null!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!MobilePhoneNumber.TryParse(Mobile, out MobilePhoneNumber phoneNumber))
            ModelState.AddModelError(nameof(Mobile), "无效的移动电话号码");

        if (!ModelState.IsValid)
            return Page();

        string code = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var normalPhoneNumber = phoneNumber.ToString();

        NaturalPerson? person = await userManager.FindByMobileAsync(normalPhoneNumber, HttpContext.RequestAborted);
        if (person is not { PhoneNumberConfirmed: true })
            //不执行操作
            return RedirectToPage("ResetPasswordMobile", new { code, phone = Mobile });

        code = await userManager.GeneratePasswordResetTokenAsync(person);

        await verificationCodeService.SendAsync(normalPhoneNumber);
        return RedirectToPage("ResetPasswordMobile", new { code, phone = Mobile });
    }
}