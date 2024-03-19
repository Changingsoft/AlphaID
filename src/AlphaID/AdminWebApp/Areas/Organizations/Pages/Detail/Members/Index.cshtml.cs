using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail.Members;

public class IndexModel(OrganizationManager manager, NaturalPersonManager personManager, OrganizationMemberManager memberManager) : PageModel
{
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
    public string UserName { get; set; } = default!;

    [BindProperty]
    [Display(Name = "MembershipVisibility")]
    public MembershipVisibility Visibility { get; set; } = MembershipVisibility.Public;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        var org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await memberManager.GetMembersAsync(org);

        return this.Page();
    }

    public async Task<IActionResult> OnPostAddMemberAsync(string anchor)
    {
        var org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await memberManager.GetMembersAsync(org);

        var person = await personManager.FindByNameAsync(this.UserName);
        if (person == null)
        {
            this.ModelState.AddModelError(nameof(this.UserName), "找不到人员");
            return this.Page();
        }

        var member = new OrganizationMember(this.Organization, person)
        {
            Title = this.Title,
            Department = this.Department,
            Remark = this.Remark,
            Visibility = this.Visibility,
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

    public async Task<IActionResult> OnPostRemoveMemberAsync(string anchor, string personId)
    {
        var org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return this.NotFound();
        this.Organization = org;
        this.Members = await memberManager.GetMembersAsync(org);

        var member = this.Members.FirstOrDefault(m => m.PersonId == personId);
        if (member == null)
            return this.Page();

        this.Result = await memberManager.LeaveOrganizationAsync(member);
        if (this.Result.Succeeded)
        {
            this.Members = await memberManager.GetMembersAsync(org);
        }
        return this.Page();
    }

}
