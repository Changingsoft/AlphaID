using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel : PageModel
{
    private readonly OrganizationManager organizationManager;

    public IndexModel(OrganizationManager organizationManager)
    {
        this.organizationManager = organizationManager;
    }


    public GenericOrganization Organization { get; set; } = default!;

    public IActionResult OnGet(string anchor)
    {
        if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
            return this.RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return this.NotFound();

        this.Organization = organization;
        return this.Page();
    }
}
