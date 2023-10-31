using DirectoryLogon;
using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail.Account;

public class DirectoryAccountsModel : PageModel
{
    private readonly NaturalPersonManager personManager;
    private readonly LogonAccountManager logonAccountManager;

    public DirectoryAccountsModel(NaturalPersonManager personManager, LogonAccountManager logonAccountManager)
    {
        this.personManager = personManager;
        this.logonAccountManager = logonAccountManager;
    }

    public NaturalPerson Person { get; set; } = default!;

    public IEnumerable<LogonAccount> LogonAccounts { get; set; } = default!;

    public async Task<IActionResult> OnGet(string anchor)
    {
        var person = await this.personManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        this.Person = person;
        this.LogonAccounts = this.logonAccountManager.GetLogonAccounts(this.Person);
        return this.Page();
    }
}
