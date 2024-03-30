using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account
{
    public class IndexModel(DirectoryAccountManager directoryAccountManager, NaturalPersonManager naturalPersonManager) : PageModel
    {
        public IEnumerable<DirectoryAccount> DirectoryAccounts { get; set; } = [];

        public async Task OnGet()
        {
            var person = await naturalPersonManager.GetUserAsync(User) ?? throw new InvalidOperationException("�Ҳ����û���");
            DirectoryAccounts = directoryAccountManager.GetLogonAccounts(person);
        }
    }
}
