using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class AuthenticationModel(RealNameManager realNameManager, NaturalPersonManager naturalPersonManager) : PageModel
{
    public RealNameAuthentication Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        NaturalPerson? person = await naturalPersonManager.GetUserAsync(User);
        if (person == null) return NotFound();

        IEnumerable<RealNameAuthentication> authentications = realNameManager.GetAuthentications(person);
        RealNameAuthentication? authentication = authentications.FirstOrDefault(a => a.Id == id);
        if (authentication == null) return NotFound();

        Data = authentication;
        return Page();
    }
}