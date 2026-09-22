using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using AuthCenterWebApp.CaptchaValidation;
using IdSubjects;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class FindPasswordByMobileModel(
    ApplicationUserManager<NaturalPerson> userManager,
    IServiceProvider serviceProvider,
    ICaptcha captcha) : PageModel
{
    /// <summary>
    /// 本页图形验证码实例 ID，与 <c>/captcha?id=</c> 及验证码图片元素的 <c>data-captcha-id</c> 一致。
    /// </summary>
    private const string CaptchaId = "FindPasswordByMobile";

    [BindProperty]
    [Display(Name = "Phone number")]
    [Required(ErrorMessage = "Validate_Required")]
    [StringLength(14, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Captcha code")]
    [Required(ErrorMessage = "Validate_Required")]
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
        //图形验证码校验。必须早于任何 return Page()，以维持视图 _CaptchaScripts 中
        //「页面重新渲染即换图」的约定。
        if (!CaptchaValidator.Validate(captcha, CaptchaId, CaptchaCode))
            ModelState.AddModelError(nameof(CaptchaCode), Resources.SharedResource.Captcha_Invalid);

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