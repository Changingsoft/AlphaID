using Microsoft.AspNetCore.Mvc;
using Organizational;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail;

public class IndexModel(OrganizationManager organizationManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = null!;

    public Organization Organization { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        Organization? org = await organizationManager.FindByIdAsync(Anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        return Page();
    }
}