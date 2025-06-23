using System.Diagnostics;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations;

public class IndexModel(OrganizationMemberManager memberManager, UserManager<NaturalPerson> personManager, OrganizationManager organizationManager) : PageModel
{
    public IEnumerable<UserMembership> Members { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);

        Members = memberManager.GetVisibleMembersOf(person.Id, person.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string organizationId)
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);
        Members = memberManager.GetVisibleMembersOf(person.Id, person.Id);
        var org = await organizationManager.FindByIdAsync(organizationId);
        if (org == null)
        {
            ModelState.AddModelError(nameof(organizationId), "Organization not found.");
            return Page();
        }
        var member = org.Members.FirstOrDefault(m => m.PersonId == person.Id);
        if (member == null)
        {
            ModelState.AddModelError(nameof(organizationId), "You are not a member of this organization.");
            return Page();
        }
        org.Members.Remove(member);
        Result = await organizationManager.UpdateAsync(org);
        return Page();
    }
}