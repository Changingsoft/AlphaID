using IdSubjects;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class RequestsModel(RealNameRequestManager realNameRequestManager, NaturalPersonManager naturalPersonManager)
    : PageModel
{
    public IEnumerable<RealNameRequest> RealNameRequests { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await naturalPersonManager.GetUserAsync(User);
        if (person == null)
            return NotFound();
        RealNameRequests = realNameRequestManager.GetRequests(person);
        return Page();
    }
}