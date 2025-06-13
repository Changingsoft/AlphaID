using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Membership;

public class IndexModel(
    UserManager<NaturalPerson> personManager,
    OrganizationManager organizationManager,
    IOrganizationMemberStore organizationMemberStore,
    OrganizationMemberManager memberManager) : PageModel
{
    public NaturalPerson Person { get; set; } = null!;

    public IEnumerable<OrganizationMember> OrganizationMembers { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        OrganizationMembers = organizationMemberStore.OrganizationMembers.Where(m => m.PersonId == anchor);
        return Page();
    }

    public async Task<IActionResult> OnPostJoinOrganizationAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        OrganizationMembers = organizationMemberStore.OrganizationMembers.Where(m => m.PersonId == anchor);

        Organization? org = await organizationManager.FindByIdAsync(Input.OrganizationId);
        if (org == null)
        {
            ModelState.AddModelError(nameof(Input.OrganizationId), "Organization Not Found.");
            return Page();
        }

        //OrganizationMember member = new(org, Person)
        //{
        //    Title = Input.Title,
        //    Department = Input.Department,
        //    Remark = Input.Remark,
        //    IsOwner = Input.IsOwner,
        //    Visibility = Input.Visibility
        //};

        try
        {
            var m = await memberManager.Join(org.Id, person.Id, Input.Visibility);
            m.Title = Input.Title;
            m.Department = Input.Department;
            m.Remark = Input.Remark;
            m.IsOwner = Input.IsOwner;
            var result1 = await memberManager.UpdateAsync(m);
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

        Result = await memberManager.Leave(organizationId, anchor, true);
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