using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class IndexModel(DirectoryAccountManager directoryAccountManager, ApplicationUserManager<ApplicationUser> applicationUserManager)
    : PageModel
{
    public ApplicationUser Person { get; set; } = null!;

    public IEnumerable<DirectoryAccount> DirectoryAccounts { get; set; } = [];

    public async Task OnGet()
    {
        Person = await applicationUserManager.GetUserAsync(User) ?? throw new InvalidOperationException("找不到用户。");
        DirectoryAccounts = directoryAccountManager.GetLogonAccounts(Person);
    }
}