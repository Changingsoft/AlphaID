using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class DeleteModel(OrganizationManager manager) : PageModel
{
    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!manager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        if (!manager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null) return NotFound();

        Result = await manager.DeleteAsync(organization);
        if (!Result.Succeeded) return Page();

        return RedirectToPage("/Index", new { area = "" });
    }
}