using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail.Members;

public class EditModel(OrganizationMemberManager memberManager, OrganizationManager organizationManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor, string personId)
    {
        Organization? org = await organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        IEnumerable<OrganizationMember> members = await memberManager.GetMembersAsync(org);
        OrganizationMember? member = members.FirstOrDefault(p => p.PersonId == personId);
        if (member == null)
            return NotFound();

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

    public async Task<IActionResult> OnPostAsync(string anchor, string personId)
    {
        Organization? org = await organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        IEnumerable<OrganizationMember> members = await memberManager.GetMembersAsync(org);
        OrganizationMember? member = members.FirstOrDefault(p => p.PersonId == personId);
        if (member == null)
            return NotFound();

        member.Title = Input.Title;
        member.Department = Input.Department;
        member.Remark = Input.Remark;
        member.IsOwner = Input.IsOwner;
        member.Visibility = Input.Visibility;

        IdOperationResult result = await memberManager.UpdateAsync(member);
        if (result.Succeeded) return RedirectToPage("Index", new { anchor });

        foreach (string error in result.Errors) ModelState.AddModelError("", error);
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

        public bool IsOwner { get; set; }

        public MembershipVisibility Visibility { get; set; } = MembershipVisibility.Private;
    }
}