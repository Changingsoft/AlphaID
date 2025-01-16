using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class IndexModel(OrganizationManager manager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdOperationResult OperationResult { get; set; } = null!;

    public IActionResult OnGet(string anchor)
    {
        if (!manager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();

        Input = new InputModel
        {
            Description = organization.Description,
            Domicile = organization.Domicile,
            Contact = organization.Contact,
            Representative = organization.Representative
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        if (!manager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        organization.Description = Input.Description;
        organization.Domicile = Input.Domicile;
        organization.Contact = Input.Contact;
        organization.Representative = Input.Representative;

        await manager.UpdateAsync(organization);
        OperationResult = IdOperationResult.Success;
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Domicile")]
        public string? Domicile { get; set; }

        [Display(Name = "Contact")]
        public string? Contact { get; set; }

        [Display(Name = "Representative")]
        public string? Representative { get; set; }
    }
}