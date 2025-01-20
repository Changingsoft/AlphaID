using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class DeleteModel(OrganizationManager manager) : PageModel
{
    public OrganizationOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        var organization = manager.FindByCurrentName(anchor);
        if (organization == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        var organization = manager.FindByCurrentName(anchor);
        if (organization == null) return NotFound();

        Result = await manager.DeleteAsync(organization);
        if (!Result.Succeeded) return Page();

        return RedirectToPage("/Index", new { area = "" });
    }
}