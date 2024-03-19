using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail.Account;

public class DirectoryAccountsModel(NaturalPersonManager personManager, DirectoryAccountManager directoryAccountManager) : PageModel
{
    public NaturalPerson Person { get; set; } = default!;

    public IEnumerable<DirectoryAccount> LogonAccounts { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        var person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        this.Person = person;
        this.LogonAccounts = directoryAccountManager.GetLogonAccounts(this.Person);
        return this.Page();
    }
}
