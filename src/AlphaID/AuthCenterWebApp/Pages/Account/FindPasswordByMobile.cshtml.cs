using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using BotDetect.Web.Mvc;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class FindPasswordByMobileModel(
    ApplicationUserManager<NaturalPerson> userManager,
    IServiceProvider serviceProvider) : PageModel
{
    [BindProperty]
    [Display(Name = "Phone number")]
    [Required(ErrorMessage = "Validate_Required")]
    [StringLength(14, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Captcha code")]
    [Required(ErrorMessage = "Validate_Required")]
    [CaptchaModelStateValidation("LoginCaptcha", ErrorMessage = "Captcha_Invalid")]
    [BindProperty]
    public string CaptchaCode { get; set; } = null!;

    public IVerificationCodeService? VerificationCodeService => serviceProvider.GetService<IVerificationCodeService>();
    public IActionResult OnGet()
    {
        if (VerificationCodeService == null)
        {
            throw new InvalidOperationException("没有为系统配置短信验证码服务。");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (!MobilePhoneNumber.TryParse(PhoneNumber, out var phoneNumber))
        {
            ModelState.AddModelError(nameof(PhoneNumber), "无效的手机号");
            return Page();
        }

        await Task.Delay(2000); // Simulate a delay to prevent brute force attack
        var person = await userManager.FindByMobileAsync(phoneNumber.ToString());
        if (person == null)
        {
            ModelState.AddModelError(nameof(PhoneNumber), "无此手机号记录");
            return Page();
        }
        //Send verification code
        await VerificationCodeService!.SendAsync(phoneNumber.ToString());

        //Set the phone number to session
        HttpContext.Session.SetString("ResetPasswordPhoneNumber", phoneNumber.ToString());
        return RedirectToPage("VerifyPhoneNumber");
    }
}