using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class DeleteModel(OrganizationManager manager, IOrganizationStore store) : PageModel
{
    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await manager.FindByNameAsync(anchor);
        if (organization == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        var organization = await manager.FindByNameAsync(anchor);
        if (organization == null) return NotFound();

        Result = await store.DeleteAsync(organization);
        if (!Result.Succeeded) return Page();

        return RedirectToPage("/Index", new { area = "" });
    }
}