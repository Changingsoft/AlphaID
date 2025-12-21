using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Organization.Pages.People;

public class IndexModel(
    OrganizationManager organizationManager,
    UserManager<NaturalPerson> personManager) : PageModel
{
    public Organizational.Organization Organization { get; set; } = null!;

    public IEnumerable<MemberViewModel> Members { get; set; } = [];

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

        Members = GetMembers(organization, visitor?.Id);
        VisitorIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.UserId == visitor.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string anchor, string personId)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        VisitorIsOwner = visitor != null && organization.Members.Any(m => m.IsOwner && m.PersonId == visitor.Id);
        if (!VisitorIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }

        var your = organization.Members.FirstOrDefault(m => m.PersonId == personId);
        if (your == null)
            return Page();

        organization.Members.Remove(your);

        Result = await organizationManager.UpdateAsync(organization);
        Members = GetMembers(organization, visitor?.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostSetOwner(string anchor, string personId)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        Members = GetMembers(organization, visitor?.Id);
        VisitorIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.UserId == visitor.Id);
        if (!VisitorIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }
        var your = organization.Members.FirstOrDefault(m => m.PersonId == personId);
        if (your == null)
            return Page();
        your.IsOwner = true;
        Result = await organizationManager.UpdateAsync(organization);
        return Page();
    }

    public async Task<IActionResult> OnPostUnsetOwner(string anchor, string personId)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Organization = organization;

        NaturalPerson? visitor = await personManager.GetUserAsync(User);

        Members = GetMembers(organization, visitor?.Id);
        VisitorIsOwner = visitor != null && Members.Any(m => m.IsOwner && m.UserId == visitor.Id);
        if (!VisitorIsOwner)
        {
            ModelState.AddModelError("", "不是企业的所有者不能执行此操作。");
            return Page();
        }
        var your = organization.Members.FirstOrDefault(m => m.PersonId == personId);
        if (your == null)
            return Page();
        your.IsOwner = false;
        Result = await organizationManager.UpdateAsync(organization);
        return Page();
    }

    private IEnumerable<MemberViewModel> GetMembers(Organizational.Organization organization, string? visitorId)
    {
        var userIds = organization.Members.VisibleMembers(visitorId).Select(m => m.PersonId).ToList();
        var users = personManager.Users.Where(u => userIds.Contains(u.Id)).Select(u => new { u.Id, u.UserName, u.Name }).ToList();
        return from member in organization.Members.VisibleMembers(visitorId)
               join user in users on member.PersonId equals user.Id
               select new MemberViewModel
               {
                   UserId = user.Id,
                   UserName = user.UserName,
                   DisplayName = user.Name,
                   Department = member.Department,
                   Title = member.Title,
                   Remark = member.Remark,
                   Visibility = member.Visibility,
                   IsOwner = member.IsOwner
               };
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

        public string? DisplayName { get; set; }
    }
}