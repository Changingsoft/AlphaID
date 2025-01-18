using AlphaIdPlatform.Identity;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class AuthenticationModel(RealNameManager<NaturalPerson> realNameManager, UserManager<NaturalPerson> applicationUserManager) : PageModel
{
    public RealNameAuthentication Data { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        NaturalPerson? person = await applicationUserManager.GetUserAsync(User);
        if (person == null) return NotFound();

        IEnumerable<RealNameAuthentication> authentications = realNameManager.GetAuthentications(person);
        RealNameAuthentication? authentication = authentications.FirstOrDefault(a => a.Id == id);
        if (authentication == null) return NotFound();

        Data = authentication;
        return Page();
    }
}