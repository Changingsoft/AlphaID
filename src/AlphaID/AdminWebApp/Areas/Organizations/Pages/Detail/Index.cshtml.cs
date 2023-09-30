using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class IndexModel : PageModel
{
    private readonly OrganizationManager organizationManager;

    public IndexModel(OrganizationManager organizationManager)
    {
        this.organizationManager = organizationManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    public GenericOrganization Organization { get; set; } = default!;




    public async Task<IActionResult> OnGetAsync()
    {
        var org = await this.organizationManager.FindByIdAsync(this.Id);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        return this.Page();
    }

}
