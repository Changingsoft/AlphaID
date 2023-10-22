using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel : PageModel
{
    private readonly OrganizationMemberManager memberManager;
    private readonly NaturalPersonManager naturalPersonManager;
    private readonly OrganizationManager organizationManager;

    public IndexModel(OrganizationMemberManager memberManager, NaturalPersonManager naturalPersonManager, OrganizationManager organizationManager)
    {
        this.memberManager = memberManager;
        this.naturalPersonManager = naturalPersonManager;
        this.organizationManager = organizationManager;
    }


    public GenericOrganization Organization { get; set; } = default!;

    public async Task<IActionResult> OnGet(string anchor)
    {
        var orgs = await this.organizationManager.SearchByNameAsync(anchor);
        if(orgs.Count() > 1)
        {
            return this.RedirectToPage("Who");
        }
        if(orgs.Any())
        {
            this.Organization = orgs.First();
            return this.Page();
        }
        var org = await this.organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return this.NotFound();

        this.Organization = org;
        return this.Page();
    }
}
