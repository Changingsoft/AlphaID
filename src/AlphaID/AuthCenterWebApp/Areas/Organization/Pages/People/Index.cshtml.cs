using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.People;

public class IndexModel(
    OrganizationMemberManager organizationMemberManager,
    IOrganizationMemberStore organizationMemberStore,
    OrganizationManager organizationManager,
    UserManager<NaturalPerson> personManager) : PageModel
{
    public AlphaIdPlatform.Subjects.Organization Organization { get; set; } = null!;

    public IEnumerable<OrganizationMember> Members { get; set; } = [];

    /// <summary>
    /// 判断当前访问者是否是组织的所有者。
    /// </summary>
    public bool VisitorIsOwner { get; set; }

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);


        Members = organizationMemberStore.OrganizationMembers.VisibleMembers(Organization.Id, visitor?.Id);
        VisitorIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string anchor, string personId)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        Members = organizationMemberStore.OrganizationMembers.VisibleMembers(Organization.Id, visitor?.Id);
        VisitorIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);

        OrganizationMember? member = Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return Page();

        if (!VisitorIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }

        Result = await organizationMemberManager.LeaveUser(organization.Id, personId);
        return Page();
    }

    public async Task<IActionResult> OnPostSetOwner(string anchor, string personId)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        Members = (organizationMemberStore.OrganizationMembers.VisibleMembers(Organization.Id, visitor?.Id)).ToList();
        VisitorIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);

        OrganizationMember? member = Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return Page();

        if (!VisitorIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }

        Result = await organizationMemberManager.SetOwner(member);
        return Page();
    }

    public async Task<IActionResult> OnPostUnsetOwner(string anchor, string personId)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        Members = (organizationMemberStore.OrganizationMembers.VisibleMembers(Organization.Id, visitor?.Id)).ToList();
        VisitorIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);

        OrganizationMember? member = Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return Page();

        if (!VisitorIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }

        Result = await organizationMemberManager.UnsetOwner(member);
        return Page();
    }

    public class MemberViewModel
    {
        public string UserName { get; set; } = null!;

        public MembershipVisibility Visibility { get; set; }

        public bool IsOwner { get; set; }

        public string UserId { get; set; } = null!;

        public string? Title { get; set; }

        public string? Department { get; set; }

        public string? Remark { get; set; }
    }
}