using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Components;

public class PeopleNavPanel(NaturalPersonManager personManager) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(personManager.Users.Count());
    }
}