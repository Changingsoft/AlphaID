using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class DirectoryAccountsModel(ApplicationUserManager<ApplicationUser> personManager, DirectoryAccountManager directoryAccountManager)
    : PageModel
{
    public ApplicationUser Person { get; set; } = null!;

    public IEnumerable<DirectoryAccount> LogonAccounts { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        ApplicationUser? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Person = person;
        LogonAccounts = directoryAccountManager.GetLogonAccounts(Person);
        return Page();
    }
}