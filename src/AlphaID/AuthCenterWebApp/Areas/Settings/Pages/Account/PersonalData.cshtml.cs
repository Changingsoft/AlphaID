using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class PersonalDataModel(NaturalPersonManager userManager) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        return user == null ? this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.") : this.Page();
    }
}
