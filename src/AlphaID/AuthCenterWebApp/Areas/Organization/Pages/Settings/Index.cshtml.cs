using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class IndexModel(OrganizationManager manager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public OrganizationOperationResult OperationResult { get; set; } = null!;

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await manager.FindByNameAsync(anchor);
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
        var organization = await manager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        organization.Description = Input.Description;
        organization.Domicile = Input.Domicile;
        organization.Contact = Input.Contact;
        organization.Representative = Input.Representative;

        await manager.UpdateAsync(organization);
        OperationResult = OrganizationOperationResult.Success;
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