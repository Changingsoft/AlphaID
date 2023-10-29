using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class DeleteModel : PageModel
{
    private readonly OrganizationManager organizationManager;

    public DeleteModel(OrganizationManager organizationManager)
    {
        this.organizationManager = organizationManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    public GenericOrganization Organization { get; set; } = default!;

    [BindProperty]
    public DeleteForm Input { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var org = await this.organizationManager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var org = await this.organizationManager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;

        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        if (this.Input.Name != this.Organization.Name)
        {
            this.ModelState.AddModelError("", "输入的组织名称不一致");
            return this.Page();
        }

        try
        {
            var result = await this.organizationManager.DeleteAsync(this.Organization);
            if (result.Succeeded)
                return this.RedirectToPage("DeleteSuccess");
            this.Result = result;
            return this.Page();
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }
    }

    public class DeleteForm
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Name { get; set; } = default!;

    }
}
