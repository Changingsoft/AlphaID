using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class PersonalDataModel(ApplicationUserManager<ApplicationUser> userManager) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        return user == null ? NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.") : Page();
    }
}