using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class PeopleNavPanel(UserManager<ApplicationUser> personManager) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View(model: personManager.Users.Count());
    }
}
