using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Membership;

public class IndexModel(
    NaturalPersonManager personManager,
    OrganizationManager organizationManager,
    OrganizationMemberManager memberManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    public NaturalPerson Person { get; set; } = default!;

    public IEnumerable<OrganizationMember> OrganizationMembers { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        OrganizationMembers = await memberManager.GetMembersOfAsync(person);
        return Page();
    }

    public async Task<IActionResult> OnPostJoinOrganizationAsync()
    {
        NaturalPerson? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        OrganizationMembers = await memberManager.GetMembersOfAsync(person);

        GenericOrganization? org = await organizationManager.FindByIdAsync(Input.OrganizationId);
        if (org == null)
        {
            ModelState.AddModelError(nameof(Input.OrganizationId), "Organization Not Found.");
            return Page();
        }

        OrganizationMember member = new(org, Person)
        {
            Title = Input.Title,
            Department = Input.Department,
            Remark = Input.Remark,
            IsOwner = Input.IsOwner,
            Visibility = Input.Visibility
        };

        IdOperationResult result = await memberManager.CreateAsync(member);
        if (!result.Succeeded)
        {
            foreach (string error in result.Errors) ModelState.AddModelError("", error);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLeaveOrganizationAsync(string organizationId)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;

        IEnumerable<OrganizationMember> members = await memberManager.GetMembersOfAsync(Person);
        OrganizationMember? member = members.FirstOrDefault(m => m.OrganizationId == organizationId);
        if (member == null)
        {
            ModelState.AddModelError("", "Membership not found.");
            return Page();
        }

        Result = await memberManager.RemoveAsync(member);
        return RedirectToPage();
    }

    public class InputModel
    {
        [Display(Name = "Organization ID")]
        [Required(ErrorMessage = "Validate_Required")]
        public string OrganizationId { get; set; } = default!;

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