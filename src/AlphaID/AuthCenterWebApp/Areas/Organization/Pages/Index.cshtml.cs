using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel(OrganizationManager organizationManager) : PageModel
{
    public IdSubjects.Organization Organization { get; set; } = null!;

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();

        Organization = organization;
        return Page();
    }
}