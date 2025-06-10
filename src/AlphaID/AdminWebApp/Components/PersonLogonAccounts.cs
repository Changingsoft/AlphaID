using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class PersonLogonAccounts(IDirectoryAccountStore store) : ViewComponent
{
    public IViewComponentResult Invoke(string personId)
    {
        IQueryable<DirectoryAccount> result = store.Accounts.Where(t => t.UserId == personId);
        return View(result);
    }
}