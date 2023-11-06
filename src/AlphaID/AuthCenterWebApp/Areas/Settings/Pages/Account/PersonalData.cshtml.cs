using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class PersonalDataModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly ILogger<PersonalDataModel> logger;

    public PersonalDataModel(
        NaturalPersonManager userManager,
        ILogger<PersonalDataModel> logger)
    {
        this.userManager = userManager;
        this.logger = logger;
    }

    public async Task<IActionResult> OnGet()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        return user == null ? this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.") : this.Page();
    }
}
