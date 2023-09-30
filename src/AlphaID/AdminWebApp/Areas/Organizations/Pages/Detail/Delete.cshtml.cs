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
    public string Id { get; set; } = default!;

    public GenericOrganization Organization { get; set; } = default!;

    [BindProperty]
    public DeleteForm Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var org = await this.organizationManager.FindByIdAsync(this.Id);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var org = await this.organizationManager.FindByIdAsync(this.Id);
        if (org == null)
            return this.NotFound();
        this.Organization = org;

        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        if (this.Input.Name != this.Organization.Name)
        {
            this.ModelState.AddModelError("", "�������֯���Ʋ�һ��");
            return this.Page();
        }

        try
        {
            await this.organizationManager.DeleteAsync(this.Organization);
            return this.RedirectToPage("DeleteSuccess");
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }
    }

    public class DeleteForm
    {
        [Display(Name = "����")]
        [Required(ErrorMessage = "{0}�Ǳ����")]
        public string Name { get; set; } = default!;

    }
}
