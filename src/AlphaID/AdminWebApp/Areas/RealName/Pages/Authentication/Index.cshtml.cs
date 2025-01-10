using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.RealName.Pages.Authentication;

public class IndexModel(RealNameManager realNameManager) : PageModel
{
    public RealNameAuthentication Data { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        RealNameAuthentication? authentication = await realNameManager.FindByIdAsync(anchor);
        if (authentication == null) return NotFound();
        Data = authentication;
        return Page();
    }
}