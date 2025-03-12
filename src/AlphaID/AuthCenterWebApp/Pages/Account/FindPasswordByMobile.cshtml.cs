using System.ComponentModel.DataAnnotations;
using System.Text;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using BotDetect.Web.Mvc;
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

    [Display(Name = "Captcha code")]
    [Required(ErrorMessage = "Validate_Required")]
    [CaptchaModelStateValidation("LoginCaptcha", ErrorMessage = "Captcha_Invalid")]
    [BindProperty]
    public string CaptchaCode { get; set; } = null!;


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

        await Task.Delay(2000); // Simulate a delay to prevent brute force attack
        var person = await userManager.FindByMobileAsync(phoneNumber.ToString());
        if (person == null)
        {
            ModelState.AddModelError(nameof(Mobile), "无此移动电话号码记录");
            return Page();
        }
        //Send verifiation code
        await verificationCodeService.SendAsync(phoneNumber.ToString());

        //Set the phone number to session
        HttpContext.Session.SetString("ResetPasswordPhoneNumber", phoneNumber.ToString());
        return RedirectToPage("VerifyPhoneNumber");
    }
}