using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class FinancialModel : PageModel
{
    private readonly OrganizationManager organizationManager;

    public FinancialModel(OrganizationManager organizationManager)
    {
        this.organizationManager = organizationManager;
    }

    public GenericOrganization Data { get; set; } = default!;

    public async Task<IActionResult> OnGet(string id)
    {
        var data = await this.organizationManager.FindByIdAsync(id);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        return this.Page();
    }
}
