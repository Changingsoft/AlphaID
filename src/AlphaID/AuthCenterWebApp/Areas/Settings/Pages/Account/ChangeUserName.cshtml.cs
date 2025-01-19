using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using AuthCenterWebApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class ChangeUserNameModel(
    UserManager<NaturalPerson> manager,
    ILogger<ChangeUserNameModel>? logger,
    SignInManager<NaturalPerson> signInManager) : PageModel
{
    [BindProperty]
    [Display(Name = "User name")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
    [PageRemote(HttpMethod = "Post", PageHandler = "CheckName", AdditionalFields = "__RequestVerificationToken")]
    public string UserName { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await manager.GetUserAsync(User);
        if (person == null)
        {
            logger?.LogWarning("从用户的登录信息无法查询到用户");
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? person = await manager.GetUserAsync(User);
        if (person == null)
        {
            logger?.LogWarning("从用户的登录信息无法查询到用户");
            return NotFound();
        }

        if (!ModelState.IsValid)
            return Page();

        Result = await manager.SetUserNameAsync(person, UserName);
        if (Result.Succeeded) await signInManager.RefreshSignInAsync(person);
        return Page();
    }

    public IActionResult OnPostCheckName(string userName)
    {
        return new JsonResult(true); //todo 用户输入用户名时实时验证用户名是否可用
    }
}