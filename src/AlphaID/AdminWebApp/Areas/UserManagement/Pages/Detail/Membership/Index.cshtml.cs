using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Membership;

public class IndexModel(
    UserManager<NaturalPerson> personManager,
    OrganizationManager organizationManager,
    OrganizationMemberManager organizationMemberManager) : PageModel
{
    public NaturalPerson Person { get; set; } = null!;

    public IEnumerable<UserMembership> OrganizationMembers { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        OrganizationMembers = organizationMemberManager.GetMembersOf(anchor);
        return Page();
    }

    public async Task<IActionResult> OnPostJoinOrganizationAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        OrganizationMembers = organizationMemberManager.GetMembersOf(anchor);

        Organization? org = await organizationManager.FindByIdAsync(Input.OrganizationId);
        if (org == null)
        {
            ModelState.AddModelError(nameof(Input.OrganizationId), "Organization Not Found.");
            return Page();
        }

        try
        {
            var m = new OrganizationMember(person.Id, Input.Visibility)
            {
                Title = Input.Title,
                Department = Input.Department,
                Remark = Input.Remark,
                IsOwner = Input.IsOwner
            };
            org.Members.Add(m);
            var result1 = await organizationManager.UpdateAsync(org);
            if(!result1.Succeeded)
            {
                foreach (string error in result1.Errors) ModelState.AddModelError("", error);
                return Page();
            }
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostLeaveOrganizationAsync(string anchor, string organizationId)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        var organization = await organizationManager.FindByIdAsync(organizationId);
        if (organization == null)
        {
            ModelState.AddModelError(nameof(organizationId), "Organization Not Found.");
            return Page();
        }
        var member = organization.Members.FirstOrDefault(m => m.PersonId == person.Id);
        if (member == null)
        {
            ModelState.AddModelError(nameof(organizationId), "You are not a member of this organization.");
            return Page();
        }
        organization.Members.Remove(member);

        Result = await organizationManager.UpdateAsync(organization);
        return RedirectToPage();
    }

    public class InputModel
    {
        [Display(Name = "Organization ID")]
        [Required(ErrorMessage = "Validate_Required")]
        public string OrganizationId { get; set; } = null!;

        [Display(Name = "Department")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string? Department { get; set; }

        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string? Title { get; set; }

        [Display(Name = "Remark")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string? Remark { get; set; }

        [Display(Name = "Owner")]
        public bool IsOwner { get; set; } = false;

        [Display(Name = "Membership visibility")]
        public MembershipVisibility Visibility { get; set; } = MembershipVisibility.Private;
    }
}