using AlphaIdPlatform.Identity;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class DirectoryAccountsModel(UserManager<NaturalPerson> personManager, DirectoryAccountManager<NaturalPerson> directoryAccountManager)
    : PageModel
{
    public NaturalPerson Person { get; set; } = null!;

    public IEnumerable<DirectoryAccount> LogonAccounts { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Person = person;
        LogonAccounts = directoryAccountManager.GetLogonAccounts(Person);
        return Page();
    }
}