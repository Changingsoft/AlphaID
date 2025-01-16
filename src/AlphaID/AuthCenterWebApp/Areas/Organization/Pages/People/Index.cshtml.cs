using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.People;

public class IndexModel(
    OrganizationMemberManager organizationMemberManager,
    OrganizationManager organizationManager,
    ApplicationUserManager personManager) : PageModel
{
    public GenericOrganization Organization { get; set; } = null!;

    public IEnumerable<OrganizationMember> Members { get; set; } = [];

    public bool UserIsOwner { get; set; }

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who");
        if (organization == null)
            return NotFound();
        Organization = organization;

        ApplicationUser? visitor = await personManager.GetUserAsync(User);


        Members = await organizationMemberManager.GetVisibleMembersAsync(Organization, visitor);
        UserIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string anchor, string personId)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who");
        if (organization == null)
            return NotFound();
        Organization = organization;

        ApplicationUser? visitor = await personManager.GetUserAsync(User);

        Members = (await organizationMemberManager.GetVisibleMembersAsync(Organization, visitor)).ToList();
        UserIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);

        OrganizationMember? member = Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return Page();

        if (!UserIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }

        Result = await organizationMemberManager.LeaveOrganizationAsync(member);
        return Page();
    }

    public async Task<IActionResult> OnPostSetOwner(string anchor, string personId)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who");
        if (organization == null)
            return NotFound();
        Organization = organization;

        ApplicationUser? visitor = await personManager.GetUserAsync(User);

        Members = (await organizationMemberManager.GetVisibleMembersAsync(Organization, visitor)).ToList();
        UserIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);

        OrganizationMember? member = Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return Page();

        if (!UserIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }

        Result = await organizationMemberManager.SetOwner(member);
        return Page();
    }

    public async Task<IActionResult> OnPostUnsetOwner(string anchor, string personId)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who");
        if (organization == null)
            return NotFound();
        Organization = organization;

        ApplicationUser? visitor = await personManager.GetUserAsync(User);

        Members = (await organizationMemberManager.GetVisibleMembersAsync(Organization, visitor)).ToList();
        UserIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);

        OrganizationMember? member = Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return Page();

        if (!UserIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }

        Result = await organizationMemberManager.UnsetOwner(member);
        return Page();
    }
}