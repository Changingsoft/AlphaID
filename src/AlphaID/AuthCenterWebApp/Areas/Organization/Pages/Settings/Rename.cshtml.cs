using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class RenameModel(OrganizationManager manager) : PageModel
{
    [BindProperty]
    [Display(Name = "Name")]
    [StringLength(50, ErrorMessage = "Validate_StringLength")]
    public string Name { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await manager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        var organization = await manager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();

        if (manager.Organizations.Any(o => o.Name == Name))
            ModelState.AddModelError(nameof(Name), "The name is in use.");

        if (!ModelState.IsValid)
            return Page();

        Result = await manager.RenameAsync(organization, Name);
        if (!Result.Succeeded)
            return Page();

        //Redirect with new name
        return RedirectToPage("/Index", new { anchor = Name });
    }
}