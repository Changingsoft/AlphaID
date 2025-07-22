using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using Duende.IdentityServer.Extensions;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class MobileModel(UserManager<NaturalPerson> userManager, IServiceProvider serviceProvider) : PageModel
{
    [Display(Name = "PhoneNumber phone number")]
    public string Mobile { get; set; } = null!;

    public bool MobileValid { get; set; }

    [Display(Name = "New mobile phone number")]
    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    public string NewMobile { get; set; } = null!;

    [Display(Name = "Verification code")]
    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    public string VerificationCode { get; set; } = null!;

    public bool VerificationCodeSent { get; set; }

    public string? OperationMessage { get; set; }

    public IVerificationCodeService? VerificationCodeService => serviceProvider.GetService<IVerificationCodeService>();

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await userManager.GetUserAsync(User);
        if (person == null)
            return BadRequest("无法处理用户Id.");

        MobileValid = person.PhoneNumberConfirmed;
        Mobile = person.PhoneNumber ?? "";
        NewMobile = person.PhoneNumber ?? "";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? person = await userManager.FindByIdAsync(User.GetSubjectId());
        if (person == null)
            return BadRequest("无法处理用户Id.");

        if (!MobilePhoneNumber.TryParse(NewMobile, out MobilePhoneNumber phoneNumber))
        {
            ModelState.AddModelError(nameof(NewMobile), "移动电话号码无效。");
            return Page();
        }

        bool phoneNumberConfirmed = false;
        if (VerificationCodeService is not null)
        {
            if (!await VerificationCodeService.VerifyAsync(phoneNumber.ToString(), VerificationCode))
            {
                ModelState.AddModelError(nameof(VerificationCode), "验证码无效。");
                return Page();
            }
            else
            {
                phoneNumberConfirmed = true;
            }
        }

        IdentityResult result = await userManager.SetPhoneNumberAsync(person, phoneNumber.ToString());
        if (!result.Succeeded)
        {
            OperationMessage = "无法变更移动电话号码。";
            return Page();
        }
        person.PhoneNumberConfirmed = phoneNumberConfirmed;
        result = await userManager.UpdateAsync(person);
        if (!result.Succeeded)
        {
            OperationMessage = "无法确认移动电话号码。";
            return Page();
        }

        OperationMessage = "移动电话号码已变更。";
        return Page();
    }

    public async Task<IActionResult> OnPostSendVerificationCode()
    {
        if (VerificationCodeService is null)
        {
            throw new InvalidOperationException("没有为系统配置短信验证码服务。");
        }
        if (!MobilePhoneNumber.TryParse(NewMobile, out MobilePhoneNumber phoneNumber))
        {
            ModelState.AddModelError(nameof(NewMobile), "移动电话号码无效。");
            return Page();
        }

        await VerificationCodeService.SendAsync(phoneNumber.ToString());

        VerificationCodeSent = true;
        return Page();
    }
}