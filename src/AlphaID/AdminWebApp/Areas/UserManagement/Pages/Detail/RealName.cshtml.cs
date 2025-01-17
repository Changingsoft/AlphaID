using AlphaIdPlatform.Identity;
using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class RealNameModel(UserManager<NaturalPerson> userManager, RealNameManager<NaturalPerson> realNameManager) : PageModel
{
    public ApplicationUser Data { get; set; } = null!;

    public IEnumerable<RealNameAuthentication> RealNameAuthentications { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        RealNameAuthentications = realNameManager.GetAuthentications(person);

        Data = person;
        return Page();
    }
}