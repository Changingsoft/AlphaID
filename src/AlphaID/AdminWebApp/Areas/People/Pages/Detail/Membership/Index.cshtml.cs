using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail.Membership;

public class IndexModel : PageModel
{
    private readonly NaturalPersonManager personMamager;
    private readonly OrganizationManager organizationManager;
    private readonly OrganizationMemberManager memberManager;

    public IndexModel(NaturalPersonManager personManager, OrganizationManager organizationManager, OrganizationMemberManager memberManager)
    {
        this.personMamager = personManager;
        this.organizationManager = organizationManager;
        this.memberManager = memberManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    public NaturalPerson Person { get; set; } = default!;

    public IEnumerable<OrganizationMember> OrganizationMembers { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await this.personMamager.FindByIdAsync(this.Anchor.ToString());
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.OrganizationMembers = await this.memberManager.GetMembersOfAsync(person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostJoinOrganizationAsync()
    {
        var person = await this.personMamager.FindByIdAsync(this.Anchor.ToString());
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.OrganizationMembers = await this.memberManager.GetMembersOfAsync(person);

        var org = await this.organizationManager.FindByIdAsync(this.Input.OrganizationId);
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
        };

        var result = await this.memberManager.CreateAsync(member);
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
        var person = await this.personMamager.FindByIdAsync(this.Anchor.ToString());
        if (person == null)
            return this.NotFound();
        this.Person = person;

        var members = await this.memberManager.GetMembersOfAsync(this.Person);
        var member = members.FirstOrDefault(m => m.OrganizationId == organizationId);
        if (member == null)
        {
            this.ModelState.AddModelError("", "Membership not found.");
            return this.Page();
        }

        this.Result = await this.memberManager.RemoveAsync(member);
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "Organization ID")]
        [Required(ErrorMessage = "Validate_Required")]
        public string OrganizationId { get; set; } = default!;

        [Display(Name = "Department")]
        [StringLength(50)]
        public string? Department { get; set; }

        [Display(Name = "Title")]
        [StringLength(50)]
        public string? Title { get; set; }

        [Display(Name = "Remark")]
        [StringLength(50)]
        public string? Remark { get; set; }

        [Display(Name = "Owner")]
        public bool IsOwner { get; set; } = false;

        [Display(Name = "Membership visibility")]
        public MembershipVisibility Visibility { get; set; } = MembershipVisibility.Private;
    }
}
