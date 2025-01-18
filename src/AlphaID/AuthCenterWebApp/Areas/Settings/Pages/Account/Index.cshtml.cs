using AlphaIdPlatform.Identity;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class IndexModel(DirectoryAccountManager<NaturalPerson> directoryAccountManager, UserManager<NaturalPerson> applicationUserManager)
    : PageModel
{
    public NaturalPerson Person { get; set; } = null!;

    public IEnumerable<DirectoryAccount> DirectoryAccounts { get; set; } = [];

    public async Task OnGet()
    {
        Person = await applicationUserManager.GetUserAsync(User) ?? throw new InvalidOperationException("找不到用户。");
        DirectoryAccounts = directoryAccountManager.GetLogonAccounts(Person);
    }
}