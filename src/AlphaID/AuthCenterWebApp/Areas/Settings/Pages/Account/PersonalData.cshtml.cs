using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class PersonalDataModel : PageModel
{
    private readonly NaturalPersonManager _userManager;
    private readonly ILogger<PersonalDataModel> _logger;

    public PersonalDataModel(
        NaturalPersonManager userManager,
        ILogger<PersonalDataModel> logger)
    {
        this._userManager = userManager;
        this._logger = logger;
    }

    public async Task<IActionResult> OnGet()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        return user == null ? this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.") : this.Page();
    }
}
