using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class RemovePasswordModel(NaturalPersonManager userManager) : PageModel
{
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Validate_Required")]
    [BindProperty]
    public string Password { get; set; } = default!;

    public IList<UserLoginInfo> Logins { get; set; } = default!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await userManager.GetUserAsync(this.User);
        return person == null ? this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await userManager.GetUserAsync(this.User);
        if (person == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }
        this.Logins = await userManager.GetLoginsAsync(person);
        if (!this.Logins.Any())
        {
            this.ModelState.AddModelError("", "您不能移除密码，因为没有任何外部登录可用，移除密码后，您将完全无法使用该账户。要移除密码，请添加至少一个外部登录。");
            this.Result = IdentityResult.Failed(new IdentityError()
            {
                Code = "Cannot remove password",
                Description = "您不能移除密码，因为没有任何外部登录可用，移除密码后，您将完全无法使用该账户。要移除密码，请添加至少一个外部登录。",
            });
            return this.Page();
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        //check password
        if (!await userManager.CheckPasswordAsync(person, this.Password))
        {
            this.ModelState.AddModelError(nameof(this.Password), "密码错误！");
            return this.Page();
        }

        this.Result = await userManager.RemovePasswordAsync(person);
        this.Password = string.Empty;
        return this.Page();
    }
}
