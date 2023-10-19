#nullable disable

using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ConfirmEmailChangeModel : PageModel
{
    private readonly NaturalPersonManager _userManager;
    private readonly SignInManager<NaturalPerson> _signInManager;

    public ConfirmEmailChangeModel(NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
    }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
    {
        if (userId == null || email == null || code == null)
        {
            return this.RedirectToPage("/Index");
        }

        var user = await this._userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await this._userManager.ChangeEmailAsync(user, email, code);
        if (!result.Succeeded)
        {
            this.StatusMessage = "更新邮件地址时出现错误。";
            return this.Page();
        }

        // In our UI email and user name are one and the same, so when we update the email
        // we need to update the user name.
        //var setUserNameResult = await this._userManager.SetUserNameAsync(user, email);
        //if (!setUserNameResult.Succeeded)
        //{
        //    this.StatusMessage = "更改账户名称时出现错误。";
        //    return this.Page();
        //}

        await this._signInManager.RefreshSignInAsync(user);
        this.StatusMessage = "感谢您确认电子邮件变更。";
        return this.Page();
    }
}
