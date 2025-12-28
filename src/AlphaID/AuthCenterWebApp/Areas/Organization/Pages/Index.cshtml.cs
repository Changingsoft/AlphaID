using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel(IOrganizationStore store) : PageModel
{
    public Organizational.Organization Organization { get; set; } = null!;

    public IActionResult OnGet(string anchor)
    {
        var organization = store.Organizations.FirstOrDefault(o => o.Name == anchor);
        if (organization == null)
            return NotFound();

        Organization = organization;
        return Page();
    }
}