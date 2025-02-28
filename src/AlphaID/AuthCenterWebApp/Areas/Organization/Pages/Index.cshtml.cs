using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel(OrganizationManager organizationManager) : PageModel
{
    public AlphaIdPlatform.Subjects.Organization Organization { get; set; } = null!;

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();

        Organization = organization;
        return Page();
    }
}