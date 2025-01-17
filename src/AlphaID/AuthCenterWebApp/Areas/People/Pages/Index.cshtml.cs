using AlphaIdPlatform.Security;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.People.Pages;

public class IndexModel(UserManager<ApplicationUser> personManager, OrganizationMemberManager organizationMemberManager)
    : PageModel
{
    public ApplicationUser Person { get; set; } = null!;

    public bool UserIsOwner { get; set; }

    public IEnumerable<OrganizationMember> Members { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        //Support both userAnchor and user ID.
        ApplicationUser? person = await personManager.FindByNameAsync(anchor)
                                ?? await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;

        ApplicationUser? visitor = await personManager.GetUserAsync(User);

        Members = organizationMemberManager.GetVisibleMembersOf(person, visitor);

        if (!User.Identity!.IsAuthenticated) return Page();

        if (User.SubjectId() == Person.Id)
            UserIsOwner = true;

        return Page();
    }
}