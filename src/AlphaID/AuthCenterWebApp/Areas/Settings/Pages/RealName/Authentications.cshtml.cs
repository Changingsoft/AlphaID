using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class AuthenticationsModel(RealNameManager realNameManager, ApplicationUserManager<ApplicationUser> applicationUserManager)
    : PageModel
{
    public IEnumerable<RealNameAuthentication> Authentications { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? person = await applicationUserManager.GetUserAsync(User);
        if (person == null)
            return NotFound();

        Authentications = realNameManager.GetAuthentications(person);
        return Page();
    }
}