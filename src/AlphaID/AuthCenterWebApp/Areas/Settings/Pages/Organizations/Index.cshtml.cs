using AlphaIdPlatform.JoinOrgRequesting;
using AlphaIdPlatform.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations;

public class IndexModel(
    OrganizationMemberManager memberManager,
    IJoinOrganizationRequestStore joinRequestStore,
    OrganizationManager organizationManager, IOrganizationStore store) : PageModel
{
    public IEnumerable<UserMembership> Members { get; set; } = [];

    public int PendingJoinRequestCount { get; set; }

    public OrganizationOperationResult? Result { get; set; }

    public IActionResult OnGet()
    {
        Members = memberManager.GetVisibleMembersOf(User.SubjectId()!, User.SubjectId());
        PendingJoinRequestCount = joinRequestStore.Requests.Pending().Count(r => r.UserId == User.SubjectId()!);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string organizationId)
    {
        Members = memberManager.GetVisibleMembersOf(User.SubjectId()!, User.SubjectId());
        var org = await store.FindByIdAsync(organizationId);
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