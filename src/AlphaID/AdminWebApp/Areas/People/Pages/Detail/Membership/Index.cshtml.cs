using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail.Membership;

public class IndexModel(NaturalPersonManager personManager, OrganizationManager organizationManager, OrganizationMemberManager memberManager) : PageModel
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
        var person = await personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.OrganizationMembers = await memberManager.GetMembersOfAsync(person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostJoinOrganizationAsync()
    {
        var person = await personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.OrganizationMembers = await memberManager.GetMembersOfAsync(person);

        var org = await organizationManager.FindByIdAsync(this.Input.OrganizationId);
        if (org == null)
        {
            this.ModelState.AddModelError(nameof(this.Input.OrganizationId), "Organization Not Found.");
            return this.Page();
        }

        OrganizationMember member = new(org, this.Person)
        {
            Title = this.Input.Title,
            Department = this.Input.Department,
            Remark = this.Input.Remark,
            IsOwner = this.Input.IsOwner,
            Visibility = this.Input.Visibility,
        };

        var result = await memberManager.CreateAsync(member);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }
        return this.RedirectToPage();
    }

    public async Task<IActionResult> OnPostLeaveOrganizationAsync(string organizationId)
    {
        var person = await personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;

        var members = await memberManager.GetMembersOfAsync(this.Person);
        var member = members.FirstOrDefault(m => m.OrganizationId == organizationId);
        if (member == null)
        {
            this.ModelState.AddModelError("", "Membership not found.");
            return this.Page();
        }

        this.Result = await memberManager.RemoveAsync(member);
        return this.RedirectToPage();
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
