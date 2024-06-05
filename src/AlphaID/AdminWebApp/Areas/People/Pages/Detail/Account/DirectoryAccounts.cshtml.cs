using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail.Account;

public class DirectoryAccountsModel(NaturalPersonManager personManager, DirectoryAccountManager directoryAccountManager)
    : PageModel
{
    public NaturalPerson Person { get; set; } = default!;

    public IEnumerable<DirectoryAccount> LogonAccounts { get; set; } = default!;

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