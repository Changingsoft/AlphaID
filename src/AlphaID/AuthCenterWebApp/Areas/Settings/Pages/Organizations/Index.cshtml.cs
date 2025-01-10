using System.Diagnostics;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations;

public class IndexModel(OrganizationMemberManager memberManager, NaturalPersonManager personManager) : PageModel
{
    public IEnumerable<OrganizationMember> Members { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);

        Members = await memberManager.GetMembersOfAsync(person);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string organizationId)
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);
        Members = await memberManager.GetMembersOfAsync(person);
        OrganizationMember? member = Members.FirstOrDefault(m => m.OrganizationId == organizationId);
        if (member == null)
            return Page();

        Result = await memberManager.LeaveOrganizationAsync(member);
        return Page();
    }
}