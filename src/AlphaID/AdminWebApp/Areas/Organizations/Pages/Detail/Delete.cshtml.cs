using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class DeleteModel(OrganizationManager organizationManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    public GenericOrganization Organization { get; set; } = default!;

    [BindProperty]
    public DeleteForm Input { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var org = await organizationManager.FindByIdAsync(Anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var org = await organizationManager.FindByIdAsync(Anchor);
        if (org == null)
            return NotFound();
        Organization = org;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.Name != Organization.Name)
        {
            ModelState.AddModelError("", "输入的组织名称不一致");
            return Page();
        }

        try
        {
            var result = await organizationManager.DeleteAsync(Organization);
            if (result.Succeeded)
                return RedirectToPage("DeleteSuccess");
            Result = result;
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
    }

    public class DeleteForm
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Name { get; set; } = default!;

    }
}
