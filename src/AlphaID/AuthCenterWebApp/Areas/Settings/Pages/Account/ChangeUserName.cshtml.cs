using AuthCenterWebApp.Services;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account
{
    public class ChangeUserNameModel(NaturalPersonManager manager, ILogger<ChangeUserNameModel>? logger, PersonSignInManager signInManager) : PageModel
    {
        [BindProperty]
        [Display(Name = "User name")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        [PageRemote(HttpMethod = "Post", PageHandler = "CheckName", AdditionalFields = "__RequestVerificationToken")]
        public string UserName { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await manager.GetUserAsync(this.User);
            if (person == null)
            {
                logger?.LogWarning("从用户的登录信息无法查询到用户");
                return this.NotFound();
            }
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var person = await manager.GetUserAsync(this.User);
            if (person == null)
            {
                logger?.LogWarning("从用户的登录信息无法查询到用户");
                return this.NotFound();
            }
            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await manager.SetUserNameAsync(person, this.UserName);
            if(this.Result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(person);
            }
            return this.Page();
        }

        public IActionResult OnPostCheckName(string userName)
        {
            return new JsonResult(true); //todo 用户输入用户名时实时验证用户名是否可用
        }
    }
}
