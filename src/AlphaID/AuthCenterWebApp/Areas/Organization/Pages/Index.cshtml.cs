using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class IndexModel : PageModel
{
    private readonly OrganizationMemberManager memberManager;
    private readonly NaturalPersonManager naturalPersonManager;

    public IndexModel(OrganizationMemberManager memberManager, NaturalPersonManager naturalPersonManager)
    {
        this.memberManager = memberManager;
        this.naturalPersonManager = naturalPersonManager;
    }

    public IEnumerable<OrganizationMember> OrganizationMembers { get; set; } = default!;

    public async Task<IActionResult> OnGet()
    {
        var user = await this.naturalPersonManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound();
        }
        this.OrganizationMembers = await this.memberManager.GetMembersOfAsync(user);
        //
        return this.Page();
    }
}
