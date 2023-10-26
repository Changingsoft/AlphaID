using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class MembersModel : PageModel
{
    private readonly OrganizationManager organizationManager;
    private readonly NaturalPersonManager personManager;
    private readonly OrganizationMemberManager memberManager;

    public MembersModel(OrganizationManager manager, NaturalPersonManager personManager, OrganizationMemberManager memberManager)
    {
        this.organizationManager = manager;
        this.personManager = personManager;
        this.memberManager = memberManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

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

    public async Task<IActionResult> OnGetAsync()
    {
        var org = await this.organizationManager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await this.memberManager.GetMembersAsync(org);

        return this.Page();
    }

    public async Task<IActionResult> OnPostAddMemberAsync()
    {
        var org = await this.organizationManager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await this.memberManager.GetMembersAsync(org);

        var person = await this.personManager.FindByIdAsync(this.PersonId);
        if (person == null)
        {
            this.ModelState.AddModelError("PhoneNumber", "找不到人员");
            return this.Page();
        }


        var result = await this.memberManager.JoinOrganizationAsync(person, this.Organization, this.Title, this.Department, this.Remark);
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

    public async Task<IActionResult> OnPostRemoveMemberAsync(string anchor, string personId)
    {
        var org = await this.organizationManager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await this.memberManager.GetMembersAsync(org);

        var person = await this.personManager.FindByIdAsync(personId);
        if (person == null)
            return this.NotFound();

        var result = await this.memberManager.LeaveOrganizationAsync(person, this.Organization);
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
