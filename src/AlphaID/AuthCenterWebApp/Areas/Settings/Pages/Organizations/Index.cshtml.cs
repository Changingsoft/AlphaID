using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using AlphaIdPlatform.JoinOrgRequesting;
using AlphaIdPlatform.Security;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations;

public class IndexModel(
    OrganizationMemberManager memberManager,
    IJoinOrganizationRequestStore joinRequestStore,
    OrganizationManager organizationManager) : PageModel
{
    public IEnumerable<UserMembership> Members { get; set; } = [];

    public int PendingJoinRequestCount { get; set; }

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Members = memberManager.GetVisibleMembersOf(User.SubjectId()!, User.SubjectId());
        PendingJoinRequestCount = joinRequestStore.Requests.Pending().Count(r => r.UserId == User.SubjectId()!);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string organizationId)
    {
        Members = memberManager.GetVisibleMembersOf(User.SubjectId()!, User.SubjectId());
        var org = await organizationManager.FindByIdAsync(organizationId);
        if (org == null)
        {
            ModelState.AddModelError(nameof(organizationId), "Organization not found.");
            return Page();
        }
        var member = org.Members.FirstOrDefault(m => m.PersonId == User.SubjectId()!);
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