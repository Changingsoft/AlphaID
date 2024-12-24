using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class PersonLogonAccounts(IQueryableLogonAccountStore store) : ViewComponent
{
    public IViewComponentResult Invoke(string personId)
    {
        IQueryable<DirectoryAccount> result = store.LogonAccounts.Where(t => t.PersonId == personId);
        return View(result);
    }
}