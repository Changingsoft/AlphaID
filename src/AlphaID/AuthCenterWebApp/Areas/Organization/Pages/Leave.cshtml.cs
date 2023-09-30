using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class LeaveModel : PageModel
{
    private readonly OrganizationMemberManager organizationMemberManager;
    private readonly OrganizationManager organizationManager;
    private readonly NaturalPersonManager naturalPersonManager;

    public LeaveModel(OrganizationMemberManager organizationMemberManager, OrganizationManager organizationManager, NaturalPersonManager naturalPersonManager)
    {
        this.organizationMemberManager = organizationMemberManager;
        this.organizationManager = organizationManager;
        this.naturalPersonManager = naturalPersonManager;
    }

    public GenericOrganization Organization { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var org = await this.organizationManager.FindByIdAsync(id);
        if (org == null)
        {
            return this.NotFound();
        }
        this.Organization = org;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var org = await this.organizationManager.FindByIdAsync(id);
        if (org == null)
        {
            return this.NotFound();
        }
        this.Organization = org;

        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var person = await this.naturalPersonManager.GetUserAsync(this.User);

        var result = await this.organizationMemberManager.LeaveOrganizationAsync(person!, this.Organization);
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }
        return this.RedirectToPage("LeaveSuccess");
    }
}
