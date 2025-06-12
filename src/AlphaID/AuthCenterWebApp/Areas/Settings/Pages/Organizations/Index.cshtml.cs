using System.Diagnostics;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Security;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations;

public class IndexModel(OrganizationMemberManager memberManager, UserManager<NaturalPerson> personManager, IOrganizationMemberStore organizationMemberStore) : PageModel
{
    public IEnumerable<OrganizationMember> Members { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);

        Members = organizationMemberStore.OrganizationMembers.Where(m => m.PersonId == User.SubjectId());
        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(string organizationId)
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);
        Members = organizationMemberStore.OrganizationMembers.Where(m => m.PersonId == User.SubjectId());

        Result = await memberManager.Leave(organizationId, person.Id);
        return Page();
    }
}