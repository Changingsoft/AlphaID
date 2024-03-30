using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class PersonLogonAccounts(IQueryableLogonAccountStore store) : ViewComponent
{
    public IViewComponentResult Invoke(string personId)
    {
        var result = store.LogonAccounts.Where(t => t.PersonId == personId);
        return View(model: result);
    }
}
