using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenterWebApp.Components;

public class UserProfileMainCard : ViewComponent
{
    private readonly NaturalPersonManager personManager;

    public UserProfileMainCard(NaturalPersonManager personManager)
    {
        this.personManager = personManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var person = await this.personManager.GetUserAsync(this.HttpContext.User);

        return this.View(model: person);
    }
}
