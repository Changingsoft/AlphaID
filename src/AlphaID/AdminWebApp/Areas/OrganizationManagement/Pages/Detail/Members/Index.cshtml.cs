using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail.Members;

public class IndexModel(
    OrganizationManager manager,
    UserManager<NaturalPerson> personManager) : PageModel
{
    public Organization Organization { get; set; } = null!;

    public IEnumerable<OrganizationMemberViewModel> Members { get; set; } = null!;


    [BindProperty]
    [Display(Name = "Department")]
    public string? Department { get; set; }

    [BindProperty]
    [Display(Name = "Title")]
    public string? Title { get; set; }

    [BindProperty]
    [Display(Name = "Remark")]
    public string? Remark { get; set; }

    [Required(ErrorMessage = "Validate_Required")]
    [BindProperty]
    public string UserName { get; set; } = null!;

    [BindProperty]
    [Display(Name = "MembershipVisibility")]
    public MembershipVisibility Visibility { get; set; } = MembershipVisibility.Public;

    public OrganizationOperationResult? Result { get; set; }

    private IEnumerable<OrganizationMemberViewModel> GetMembers(Organization organization)
    {
        var userIds = organization.Members.Select(m => m.PersonId).ToList();
        var users = personManager.Users.Where(u => userIds.Contains(u.Id)).Select(u => new { u.Id, u.UserName, u.Name }).ToList();
        return from member in organization.Members
               join user in users on member.PersonId equals user.Id
               select new OrganizationMemberViewModel
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

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        Members = GetMembers(org);

        return Page();
    }

    public async Task<IActionResult> OnPostAddMemberAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        Members = GetMembers(org);

        NaturalPerson? person = await personManager.FindByNameAsync(UserName);
        if (person == null)
        {
            ModelState.AddModelError(nameof(UserName), "找不到人员");
            return Page();
        }
        if (org.Members.Any(m => m.PersonId == person.Id))
        {
            ModelState.AddModelError(nameof(UserName), "该人员已是组织成员");
            return Page();
        }

        var member = new OrganizationMember(person.Id, Visibility)
        {
            Title = Title,
            Department = Department,
            Remark = Remark
        };
        org.Members.Add(member);
        await manager.UpdateAsync(org);
        return RedirectToPage();

    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(string anchor, string personId)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        Members = GetMembers(org);

        var member = org.Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
        {
            ModelState.AddModelError(nameof(personId), "找不到该成员");
            return Page();
        }
        org.Members.Remove(member);
        await manager.UpdateAsync(org);
        return Page();
    }

    public class OrganizationMemberViewModel
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Department { get; set; }
        public string? Title { get; set; }
        public string? Remark { get; set; }
        public MembershipVisibility Visibility { get; set; }
        public bool IsOwner { get; set; }
    }
}