using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly NaturalPersonManager _userManager;

    public ResetPasswordModel(NaturalPersonManager userManager)
    {
        this._userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IActionResult OnGet(string code)
    {
        if (code == null)
        {
            return this.BadRequest("A code must be supplied for password reset.");
        }
        else
        {
            this.Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };
            return this.Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = await this._userManager.FindByEmailAsync(this.Input.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return this.RedirectToPage("./ResetPasswordConfirmation");
        }

        var result = await this._userManager.ResetPasswordAsync(user, this.Input.Code, this.Input.Password);
        if (result.Succeeded)
        {
            user.PasswordLastSet = DateTime.Now;
            await this._userManager.UpdateAsync(user);
            return this.RedirectToPage("./ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "{0}是必需的")]
        [EmailAddress(ErrorMessage = "{0}的格式错误")]
        public string Email { get; set; } = default!;

        [Display(Name = "新密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}是必需的")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "{0}的长度介于{2}到{1}个字符")]
        public string Password { get; set; } = default!;

        [Display(Name = "确认密码")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "{0}的长度介于{2}到{1}个字符")]
        [Compare("Password", ErrorMessage = "确认密码与新密码不一致。")]
        public string ConfirmPassword { get; set; } = default!;

        [Required(ErrorMessage = "{0}是必需的")]
        public string Code { get; set; } = default!;

    }
}
