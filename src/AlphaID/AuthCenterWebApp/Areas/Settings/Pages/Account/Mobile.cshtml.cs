using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Platform;
using Duende.IdentityServer.Extensions;
using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class MobileModel(NaturalPersonManager userManager, IVerificationCodeService verificationCodeService) : PageModel
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

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await userManager.FindByIdAsync(User.GetSubjectId());
        if (person == null)
            return BadRequest("�޷������û�Id.");

        MobileValid = person.PhoneNumberConfirmed;
        Mobile = person.PhoneNumber ?? "";
        NewMobile = person.PhoneNumber ?? "";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? person = await userManager.FindByIdAsync(User.GetSubjectId());
        if (person == null)
            return BadRequest("�޷������û�Id.");

        if (!MobilePhoneNumber.TryParse(NewMobile, out MobilePhoneNumber phoneNumber))
        {
            ModelState.AddModelError(nameof(NewMobile), "�ƶ��绰������Ч��");
            return Page();
        }

        if (!await verificationCodeService.VerifyAsync(phoneNumber.ToString(), VerificationCode))
        {
            ModelState.AddModelError(nameof(VerificationCode), "��֤����Ч��");
            return Page();
        }

        IdentityResult result = await userManager.SetPhoneNumberAsync(person, NewMobile);
        if (result.Succeeded)
        {
            OperationMessage = "�ƶ��绰�����ѱ����";
            return Page();
        }

        OperationMessage = "�޷�����ƶ��绰���롣";
        return Page();
    }

    public async Task<IActionResult> OnPostSendVerificationCode()
    {
        if (!MobilePhoneNumber.TryParse(NewMobile, out MobilePhoneNumber phoneNumber))
        {
            ModelState.AddModelError(nameof(NewMobile), "�ƶ��绰������Ч��");
            return Page();
        }

        await verificationCodeService.SendAsync(phoneNumber.ToString());

        VerificationCodeSent = true;
        return Page();
    }
}