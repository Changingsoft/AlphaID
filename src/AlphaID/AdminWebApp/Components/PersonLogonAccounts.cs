using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class PersonLogonAccounts : ViewComponent
{
    private readonly IQueryableLogonAccountStore store;

    public PersonLogonAccounts(IQueryableLogonAccountStore store)
    {
        this.store = store;
    }

    public IViewComponentResult Invoke(string personId)
    {
        var result = this.store.LogonAccounts.Where(t => t.PersonId == personId);
        return this.View(model: result);
    }
}
