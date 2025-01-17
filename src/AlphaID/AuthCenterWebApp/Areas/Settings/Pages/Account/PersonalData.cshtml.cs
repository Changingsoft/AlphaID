using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class PersonalDataModel(UserManager<ApplicationUser> userManager) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        return user == null ? NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.") : Page();
    }
}