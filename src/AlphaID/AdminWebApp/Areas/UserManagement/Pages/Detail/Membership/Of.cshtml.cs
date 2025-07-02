using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using AspNetWebLib.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Membership;

public class OfModel(UserManager<NaturalPerson> applicationUserManager, OrganizationManager organizationManager) : PageModel
{
    public OrganizationMember Member { get; set; } = null!;

    public Organization Organization { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public OrganizationOperationResult? OperationResult { get; set; }

    public IEnumerable<SelectListItem> MembershipVisibilities { get; set; } =
        EnumHelper.GetSelectListItems<MembershipVisibility>();

    public async Task<IActionResult> OnGetAsync(string anchor, string orgId)
    {
        NaturalPerson? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        var org = await organizationManager.FindByIdAsync(orgId);
        if (org == null)
            return NotFound();
        Organization = org;
        OrganizationMember? member = org.Members.FirstOrDefault(m => m.PersonId == anchor);
        if (member == null)
            return NotFound();
        Member = member;
        Input = new InputModel
        {
            Title = member.Title,
            Department = member.Department,
            Remark = member.Remark,
            IsOwner = member.IsOwner,
            Visibility = member.Visibility
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor, string orgId)
    {
        NaturalPerson? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        var org = await organizationManager.FindByIdAsync(orgId);
        if (org == null)
            return NotFound();
        Organization = org;
        var member = org.Members.FirstOrDefault(p => p.PersonId == anchor);
        if (member == null)
            return NotFound();
        Member = member;
        Member.Title = Input.Title;
        Member.Department = Input.Department;
        Member.Remark = Input.Remark;
        Member.IsOwner = Input.IsOwner;
        Member.Visibility = Input.Visibility;

        OperationResult = await organizationManager.UpdateAsync(org);

        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string? Title { get; set; }

        [Display(Name = "Department")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string? Department { get; set; }

        [Display(Name = "Remark")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string? Remark { get; set; }

        [Display(Name = "Is owner",
            Description = "The owner of organization can fully mange organization by themselves.")]
        public bool IsOwner { get; set; }

        [Display(Name = "Membership visibility")]
        public MembershipVisibility Visibility { get; set; }
    }
}