using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class WhoModel(OrganizationManager organizationManager) : PageModel
{
    public GenericOrganization[] Organizations { get; set; } = [];

    public IActionResult OnGet(string anchor)
    {
        Organizations = organizationManager.FindByName(anchor).ToArray();
        if (Organizations.Length == 0)
            return NotFound();

        if (Organizations.Length == 1) return RedirectToPage("Index", new { anchor = Organizations[0].Name });
        return Page();
    }
}