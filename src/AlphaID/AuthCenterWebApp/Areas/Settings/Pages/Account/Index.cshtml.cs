using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class IndexModel(DirectoryAccountManager directoryAccountManager, ApplicationUserManager applicationUserManager)
    : PageModel
{
    public ApplicationUser Person { get; set; } = null!;

    public IEnumerable<DirectoryAccount> DirectoryAccounts { get; set; } = [];

    public async Task OnGet()
    {
        Person = await applicationUserManager.GetUserAsync(User) ?? throw new InvalidOperationException("�Ҳ����û���");
        DirectoryAccounts = directoryAccountManager.GetLogonAccounts(Person);
    }
}