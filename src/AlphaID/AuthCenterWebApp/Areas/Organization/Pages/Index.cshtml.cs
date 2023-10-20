using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel : PageModel
{
    private readonly OrganizationMemberManager memberManager;
    private readonly NaturalPersonManager naturalPersonManager;
    private readonly OrganizationManager organizationManager;

    public IndexModel(OrganizationMemberManager memberManager, NaturalPersonManager naturalPersonManager, OrganizationManager organizationManager)
    {
        this.memberManager = memberManager;
        this.naturalPersonManager = naturalPersonManager;
        this.organizationManager = organizationManager;
    }


    public GenericOrganization Organization { get; set; } = default!;

    public async Task<IActionResult> OnGet(string id)
    {
        var user = await this.naturalPersonManager.GetUserAsync(this.User);
        if (user == null)
            return this.NotFound();
        var members = await this.memberManager.GetMembersOfAsync(user);
        var org = members.FirstOrDefault(m => m.OrganizationId == id);
        if (org == null)
            return this.NotFound();
        this.Organization = org.Organization;
        return this.Page();
    }
}
