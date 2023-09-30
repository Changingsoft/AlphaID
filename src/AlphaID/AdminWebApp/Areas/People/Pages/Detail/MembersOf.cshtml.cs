using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class MembersOfModel : PageModel
{
    private readonly NaturalPersonManager personMamager;
    private readonly OrganizationManager organizationManager;
    private readonly OrganizationMemberManager memberManager;

    public MembersOfModel(NaturalPersonManager personManager, OrganizationManager organizationManager, OrganizationMemberManager memberManager)
    {
        this.personMamager = personManager;
        this.organizationManager = organizationManager;
        this.memberManager = memberManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    public NaturalPerson Person { get; set; } = default!;

    public IEnumerable<OrganizationMember> OrganizationMembers { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "{0}是必需的")]
    [Display(Name = "组织Id")]
    public string OrganizationId { get; set; } = default!;

    [BindProperty]
    [MaxLength(50)]
    [Display(Name = "部门")]
    public string? Department { get; set; }

    [BindProperty]
    [MaxLength(50)]
    [Display(Name = "职务")]
    public string? Title { get; set; }

    [BindProperty]
    [MaxLength(50)]
    [Display(Name = "备注")]
    public string? Remark { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await this.personMamager.FindByIdAsync(this.Id.ToString());
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.OrganizationMembers = await this.memberManager.GetMembersOfAsync(person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostJoinOrganizationAsync()
    {
        var person = await this.personMamager.FindByIdAsync(this.Id.ToString());
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.OrganizationMembers = await this.memberManager.GetMembersOfAsync(person);

        var org = await this.organizationManager.FindByIdAsync(this.OrganizationId);
        if (org == null)
        {
            this.ModelState.AddModelError(nameof(this.OrganizationId), "Organization Not Found.");
            return this.Page();
        }

        var result = await this.memberManager.JoinOrganizationAsync(person, org, this.Department, this.Title, this.Remark);
        if (!result.IsSuccess)
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
        var person = await this.personMamager.FindByIdAsync(this.Id.ToString());
        if (person == null)
            return this.NotFound();
        this.Person = person;

        var org = await this.organizationManager.FindByIdAsync(organizationId);
        if (org == null)
        {
            this.ModelState.AddModelError("", "Organization Not Found.");
            return this.Page();
        }

        var result = await this.memberManager.LeaveOrganizationAsync(person, org);
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
        }
        return this.RedirectToPage();
    }
}
