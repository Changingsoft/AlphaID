using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail;

public class DeleteModel(OrganizationManager organizationManager) : PageModel
{
    public Organization Organization { get; set; } = null!;

    [BindProperty]
    public DeleteForm Input { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? org = await organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        Organization? org = await organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;

        if (!ModelState.IsValid) return Page();

        if (Input.Name != Organization.Name)
        {
            ModelState.AddModelError("", "输入的组织名称不一致");
            return Page();
        }

        try
        {
            OrganizationOperationResult result = await organizationManager.DeleteAsync(Organization);
            if (result.Succeeded)
                return RedirectToPage("DeleteSuccess", new { anchor });
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
        public string Name { get; set; } = null!;
    }
}