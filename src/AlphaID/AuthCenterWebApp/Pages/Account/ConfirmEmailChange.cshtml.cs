#nullable disable

using System.Text;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ConfirmEmailChangeModel(NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager)
    : PageModel
{
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
    {
        if (userId == null || email == null || code == null) return RedirectToPage("/Index");

        NaturalPerson user = await userManager.FindByIdAsync(userId);
        if (user == null) return NotFound($"Unable to load user with ID '{userId}'.");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult result = await userManager.ChangeEmailAsync(user, email, code);
        if (!result.Succeeded)
        {
            StatusMessage = "更新邮件地址时出现错误。";
            return Page();
        }

        // In our UI email and user name are one and the same, so when we update the email
        // we need to update the user name.
        //var setUserNameResult = await this._userManager.SetUserNameAsync(user, email);
        //if (!setUserNameResult.Succeeded)
        //{
        //    this.StatusMessage = "更改账户名称时出现错误。";
        //    return this.Page();
        //}

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = "感谢您确认电子邮件变更。";
        return Page();
    }
}