using AlphaIdPlatform.Identity;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class AuthenticationsModel(RealNameManager<NaturalPerson> realNameManager, UserManager<NaturalPerson> applicationUserManager)
    : PageModel
{
    public IEnumerable<RealNameAuthentication> Authentications { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await applicationUserManager.GetUserAsync(User);
        if (person == null)
            return NotFound();

        Authentications = realNameManager.GetAuthentications(person);
        return Page();
    }
}