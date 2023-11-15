using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Components;

public class PeopleNavPanel : ViewComponent
{
    private readonly NaturalPersonManager personManager;

    public PeopleNavPanel(NaturalPersonManager personManager)
    {
        this.personManager = personManager;
    }

    public IViewComponentResult Invoke()
    {
        return this.View(model: this.personManager.Users.Count());
    }
}
