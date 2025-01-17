using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Components;

public class PeopleNavPanel(UserManager<ApplicationUser> personManager) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(personManager.Users.Count());
    }
}