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
                this.logger?.LogWarning("���û��ĵ�¼��Ϣ�޷���ѯ���û�");
                return this.NotFound();
            }
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var person = await this.manager.GetUserAsync(this.User);
            if (person == null)
            {
                this.logger?.LogWarning("���û��ĵ�¼��Ϣ�޷���ѯ���û�");
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
            return new JsonResult(true); //todo �û������û���ʱʵʱ��֤�û����Ƿ����
            return new JsonResult(this.localizer["Invalid user name"].ToString());
        }
    }
}
