using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel(OrganizationManager organizationManager) : PageModel
{
    public GenericOrganization Organization { get; set; } = default!;

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
            return this.RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return this.NotFound();

        this.Organization = organization;
        return this.Page();
    }
}
