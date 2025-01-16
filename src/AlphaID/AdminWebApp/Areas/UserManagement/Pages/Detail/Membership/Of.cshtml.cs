using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Helpers;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Membership;

public class OfModel(OrganizationMemberManager memberManager, ApplicationUserManager applicationUserManager) : PageModel
{
    public OrganizationMember Member { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdOperationResult? OperationResult { get; set; }

    public IEnumerable<SelectListItem> MembershipVisibilities { get; set; } =
        EnumHelper.GetSelectListItems<MembershipVisibility>();

    public async Task<IActionResult> OnGetAsync(string anchor, string orgId)
    {
        ApplicationUser? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        IEnumerable<OrganizationMember> members = await memberManager.GetMembersOfAsync(person);
        OrganizationMember? member = members.FirstOrDefault(p => p.OrganizationId == orgId);
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
        ApplicationUser? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        IEnumerable<OrganizationMember> members = await memberManager.GetMembersOfAsync(person);
        OrganizationMember? member = members.FirstOrDefault(p => p.OrganizationId == orgId);
        if (member == null)
            return NotFound();
        Member = member;

        member.Title = Input.Title;
        member.Department = Input.Department;
        member.Remark = Input.Remark;
        member.IsOwner = Input.IsOwner;
        member.Visibility = Input.Visibility;

        OperationResult = await memberManager.UpdateAsync(member);

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