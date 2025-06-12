using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail.Members;

public class IndexModel(
    OrganizationManager manager,
    UserManager<NaturalPerson> personManager,
    IOrganizationMemberStore organizationMemberStore,
    OrganizationMemberManager memberManager) : PageModel
{
    public Organization Organization { get; set; } = null!;

    public IEnumerable<OrganizationMember> Members { get; set; } = null!;


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
    public string UserName { get; set; } = null!;

    [BindProperty]
    [Display(Name = "MembershipVisibility")]
    public MembershipVisibility Visibility { get; set; } = MembershipVisibility.Public;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        Members = organizationMemberStore.OrganizationMembers.Where(m => m.OrganizationId == anchor);

        return Page();
    }

    public async Task<IActionResult> OnPostAddMemberAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        Members = organizationMemberStore.OrganizationMembers.Where(m => m.OrganizationId == anchor);

        NaturalPerson? person = await personManager.FindByNameAsync(UserName);
        if (person == null)
        {
            ModelState.AddModelError(nameof(UserName), "找不到人员");
            return Page();
        }

        var member = new OrganizationMember(Organization, person)
        {
            Title = Title,
            Department = Department,
            Remark = Remark,
            Visibility = Visibility
        };

        OrganizationOperationResult result = await memberManager.CreateAsync(member);
        if (!result.Succeeded)
        {
            foreach (string error in result.Errors) ModelState.AddModelError("", error);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(string anchor, string personId)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        Organization = org;
        Members = organizationMemberStore.OrganizationMembers.Where(m => m.OrganizationId == anchor);

        Result = await memberManager.LeaveUser(anchor, personId);

        if (Result.Succeeded) Members = organizationMemberStore.OrganizationMembers.Where(m => m.OrganizationId == anchor);
        return Page();
    }
}