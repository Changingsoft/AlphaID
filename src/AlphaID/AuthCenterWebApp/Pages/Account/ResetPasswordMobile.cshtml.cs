using System.ComponentModel.DataAnnotations;
using System.Text;
using AlphaIdPlatform.Identity;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ResetPasswordMobileModel(UserManager<NaturalPerson> userManager) : PageModel
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
        if (!MobilePhoneNumber.TryParse(Input.PhoneNumber, out var phone))
        {
            ModelState.AddModelError(nameof(Input.PhoneNumber), "移动电话号码无效");
        }

        if (!ModelState.IsValid)
            return Page();

        var normalPhoneNumber = phone.ToString();
        var person = userManager.Users.FirstOrDefault(p => p.PhoneNumber == normalPhoneNumber);
        if (person == null || !person.PhoneNumberConfirmed)
        {
            return RedirectToPage("ResetPasswordConfirmation");
        }

        var result = await userManager.ResetPasswordAsync(person, Input.Code, Input.NewPassword);
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