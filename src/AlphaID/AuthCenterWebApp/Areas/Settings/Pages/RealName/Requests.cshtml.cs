using IdSubjects;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class RequestsModel(RealNameRequestManager realNameRequestManager, ApplicationUserManager applicationUserManager)
    : PageModel
{
    public IEnumerable<RealNameRequest> RealNameRequests { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? person = await applicationUserManager.GetUserAsync(User);
        if (person == null)
            return NotFound();
        RealNameRequests = realNameRequestManager.GetRequests(person);
        return Page();
    }
}