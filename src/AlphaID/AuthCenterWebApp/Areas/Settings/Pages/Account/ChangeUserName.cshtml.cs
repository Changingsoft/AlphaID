using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account
{
    public class ChangeUserNameModel : PageModel
    {
        NaturalPersonManager manager;
        IStringLocalizer<SharedResource> localizer;
        ILogger<ChangeUserNameModel>? logger;

        public ChangeUserNameModel(NaturalPersonManager manager, IStringLocalizer<SharedResource> localizer, ILogger<ChangeUserNameModel>? logger)
        {
            this.manager = manager;
            this.localizer = localizer;
            this.logger = logger;
        }

        [BindProperty]
        [Display(Name = "User name")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        [PageRemote(HttpMethod = "Post", PageHandler = "CheckName", AdditionalFields = "__RequestVerificationToken")]
        public string UserName { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await this.manager.GetUserAsync(this.User);
            if (person == null)
            {
                this.logger?.LogWarning("从用户的登录信息无法查询到用户");
                return this.NotFound();
            }
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var person = await this.manager.GetUserAsync(this.User);
            if (person == null)
            {
                this.logger?.LogWarning("从用户的登录信息无法查询到用户");
                return this.NotFound();
            }
            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await this.manager.SetUserNameAsync(person, this.UserName);
            //this.manager.userna
            return this.Page();
        }

        public async Task<IActionResult> OnPostCheckNameAsync(string userName)
        {
            return new JsonResult(true); //todo 用户输入用户名时实时验证用户名是否可用
            return new JsonResult(this.localizer["Invalid user name"].ToString());
        }
    }
}
