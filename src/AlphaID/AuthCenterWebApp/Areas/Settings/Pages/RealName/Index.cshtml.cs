using AlphaIdPlatform.Identity;
using IdSubjects.RealName;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class IndexModel(
    UserManager<NaturalPerson> personManager,
    RealNameManager<NaturalPerson> realNameManager,
    RealNameRequestManager<NaturalPerson> realNameRequestManager) : PageModel
{
    public IEnumerable<RealNameAuthentication> Authentications { get; set; } = null!;

    public IEnumerable<RealNameRequest> Requests { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        if (person == null) return NotFound();

        Authentications = realNameManager.GetAuthentications(person);
        Requests = realNameRequestManager.GetRequests(person).Where(r => !r.Accepted.HasValue);
        return Page();
    }
}