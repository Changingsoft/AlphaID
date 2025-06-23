using System.ComponentModel.DataAnnotations;
using System.Text;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ResetPasswordMobileModel(NaturalPersonService naturalPersonService, ApplicationUserManager<NaturalPerson> userManager, ILogger<ResetPasswordMobileModel>? logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IActionResult OnGet(string code, string phone)
    {
        Input = new InputModel
        {
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
            PhoneNumber = phone
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!MobilePhoneNumber.TryParse(Input.PhoneNumber, out _))
        {
            ModelState.AddModelError(nameof(Input.PhoneNumber), "移动电话号码无效");
        }

        if (!ModelState.IsValid)
            return Page();

        var person = await userManager.FindByMobileAsync(Input.PhoneNumber);
        if (person == null)
        {
            logger?.LogWarning("未登录用户输入手机号{PhoneNumber}，但找不到已注册的用户。", Input.PhoneNumber);
            return RedirectToPage("ResetPasswordConfirmation");
        }

        if (!person.PhoneNumberConfirmed)
        {
            logger?.LogWarning("用户{User}通过手机号重置密码，但手机号未确认。", person);
            return RedirectToPage("ResetPasswordConfirmation");
        }

        var result = await naturalPersonService.ResetPasswordAsync(person, Input.Code, Input.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                return Page();
            }
        }

        return RedirectToPage("ResetPasswordConfirmation");
    }

    public class InputModel
    {
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "{0}是必需的")]
        [Display(Name = "移动电话号码")]
        public string PhoneNumber { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}是必需的")]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}是必需的")]
        [Compare(nameof(NewPassword))]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; } = null!;
    }
}