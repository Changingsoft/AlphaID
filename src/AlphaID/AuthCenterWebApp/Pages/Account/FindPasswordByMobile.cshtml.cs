using System.ComponentModel.DataAnnotations;
using System.Text;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class FindPasswordByMobileModel(
    ApplicationUserManager<NaturalPerson> userManager,
    IVerificationCodeService verificationCodeService) : PageModel
{
    [Display(Name = "Phone number")]
    [Required(ErrorMessage = "Validate_Required")]
    [BindProperty]
    public string Mobile { get; set; } = null!;

    [Display(Name = "验证码")]
    [Required(ErrorMessage = "{0}是必需的")]
    [BindProperty]
    public string VerificationCode { get; set; } = null!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (!MobilePhoneNumber.TryParse(Mobile, out var phoneNumber))
        {
            ModelState.AddModelError(nameof(Mobile), "无效的移动电话号码");
            return Page();
        }

        if (!await verificationCodeService.VerifyAsync(phoneNumber.ToString(), VerificationCode))
        {
            ModelState.AddModelError(nameof(VerificationCode), "无效的验证码");
            return Page();
        }

        var person = userManager.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber.ToString());
        if (person == null)
        {
            ModelState.AddModelError(nameof(Mobile), "无此移动电话号码记录");
            return Page();
        }
        var code = await userManager.GeneratePasswordResetTokenAsync(person);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return RedirectToPage("ResetPasswordMobile", new { code, phone = phoneNumber.PhoneNumber });
    }

    public async Task<IActionResult> OnPostSendVerificationCodeAsync(string mobile)
    {
        if (!MobilePhoneNumber.TryParse(mobile, out var phoneNumber))
        {
            return new JsonResult("移动电话号码无效。");
        }
        await verificationCodeService.SendAsync(phoneNumber.ToString());
        return new JsonResult(true);
    }
}