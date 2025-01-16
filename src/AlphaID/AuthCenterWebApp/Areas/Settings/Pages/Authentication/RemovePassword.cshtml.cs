using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class RemovePasswordModel(NaturalPersonService naturalPersonService, ApplicationUserManager<ApplicationUser> userManager) : PageModel
{
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Validate_Required")]
    [BindProperty]
    public string Password { get; set; } = null!;

    public IList<UserLoginInfo> Logins { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? person = await userManager.GetUserAsync(User);
        return person == null ? NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.") : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUser? person = await userManager.GetUserAsync(User);
        if (person == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        Logins = await userManager.GetLoginsAsync(person);
        if (!Logins.Any())
        {
            ModelState.AddModelError("", "您不能移除密码，因为没有任何外部登录可用，移除密码后，您将完全无法使用该账户。要移除密码，请添加至少一个外部登录。");
            Result = IdentityResult.Failed(new IdentityError
            {
                Code = "Cannot remove password",
                Description = "您不能移除密码，因为没有任何外部登录可用，移除密码后，您将完全无法使用该账户。要移除密码，请添加至少一个外部登录。"
            });
            return Page();
        }

        if (!ModelState.IsValid)
            return Page();

        //check password
        if (!await userManager.CheckPasswordAsync(person, Password))
        {
            ModelState.AddModelError(nameof(Password), "密码错误！");
            return Page();
        }

        Result = await naturalPersonService.RemovePasswordAsync(person);
        Password = string.Empty;
        return Page();
    }
}