using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class IndexModel(OrganizationManager organizationManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    public GenericOrganization Organization { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        GenericOrganization? org = await organizationManager.FindByIdAsync(Anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        return Page();
    }
}