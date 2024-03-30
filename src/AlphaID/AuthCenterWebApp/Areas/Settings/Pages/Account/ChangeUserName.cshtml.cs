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
            var person = await manager.GetUserAsync(User);
            if (person == null)
            {
                logger?.LogWarning("���û��ĵ�¼��Ϣ�޷���ѯ���û�");
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var person = await manager.GetUserAsync(User);
            if (person == null)
            {
                logger?.LogWarning("���û��ĵ�¼��Ϣ�޷���ѯ���û�");
                return NotFound();
            }
            if (!ModelState.IsValid)
                return Page();

            Result = await manager.SetUserNameAsync(person, UserName);
            if(Result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(person);
            }
            return Page();
        }

        public IActionResult OnPostCheckName(string userName)
        {
            return new JsonResult(true); //todo �û������û���ʱʵʱ��֤�û����Ƿ����
        }
    }
}
