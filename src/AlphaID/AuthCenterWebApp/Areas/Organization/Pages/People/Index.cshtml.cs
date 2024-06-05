using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.People;

public class IndexModel(
    OrganizationMemberManager organizationMemberManager,
    OrganizationManager organizationManager,
    NaturalPersonManager personManager) : PageModel
{
    public GenericOrganization Organization { get; set; } = default!;

    public IEnumerable<OrganizationMember> Members { get; set; } = [];

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who");
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        Members = await organizationMemberManager.GetVisibleMembersAsync(Organization, visitor);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string anchor, string personId)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who");
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        Members = await organizationMemberManager.GetVisibleMembersAsync(Organization, visitor);

        OrganizationMember? member = Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return Page();

        Result = await organizationMemberManager.LeaveOrganizationAsync(member);
        return Page();
    }
}