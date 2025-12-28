using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class DeleteModel(IOrganizationStore store) : PageModel
{
    public OrganizationOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        var organization = store.Organizations.FirstOrDefault(o => o.Name == anchor);
        if (organization == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        var organization = store.Organizations.FirstOrDefault(o => o.Name == anchor);
        if (organization == null) return NotFound();

        Result = await store.DeleteAsync(organization);
        if (!Result.Succeeded) return Page();

        return RedirectToPage("/Index", new { area = "" });
    }
}