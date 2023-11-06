using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail.Members;

public class IndexModel : PageModel
{
    private readonly OrganizationManager organizationManager;
    private readonly NaturalPersonManager personManager;
    private readonly OrganizationMemberManager memberManager;

    public IndexModel(OrganizationManager manager, NaturalPersonManager personManager, OrganizationMemberManager memberManager)
    {
        this.organizationManager = manager;
        this.personManager = personManager;
        this.memberManager = memberManager;
    }

    public GenericOrganization Organization { get; set; } = default!;

    public IEnumerable<OrganizationMember> Members { get; set; } = default!;


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
    public string PersonId { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        var org = await this.organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await this.memberManager.GetMembersAsync(org);

        return this.Page();
    }

    public async Task<IActionResult> OnPostAddMemberAsync(string anchor)
    {
        var org = await this.organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await this.memberManager.GetMembersAsync(org);

        var person = await this.personManager.FindByIdAsync(this.PersonId);
        if (person == null)
        {
            this.ModelState.AddModelError(nameof(this.PersonId), "找不到人员");
            return this.Page();
        }

        var member = new OrganizationMember(this.Organization, person)
        {
            Title = this.Title,
            Department = this.Department,
            Remark = this.Remark,
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

    public async Task<IActionResult> OnPostRemoveMemberAsync(string anchor, string personId)
    {
        var org = await this.organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await this.memberManager.GetMembersAsync(org);

        var person = await this.personManager.FindByIdAsync(personId);
        if (person == null)
            return this.NotFound();

        this.Result = await this.memberManager.LeaveOrganizationAsync(person, this.Organization);
        if (this.Result.Succeeded)
        {
            this.Members = await this.memberManager.GetMembersAsync(org);
        }
        return this.Page();
    }

}
