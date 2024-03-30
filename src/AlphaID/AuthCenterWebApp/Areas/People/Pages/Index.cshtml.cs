using AlphaIdPlatform.Security;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.People.Pages
{
    public class IndexModel(NaturalPersonManager personManager, OrganizationMemberManager organizationMemberManager) : PageModel
    {
        public NaturalPerson Person { get; set; } = default!;

        public bool UserIsOwner { get; set; }

        public IEnumerable<OrganizationMember> Members { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            //Support both userAnchor and user ID.
            var person = await personManager.FindByNameAsync(anchor)
                ?? await personManager.FindByIdAsync(anchor);
            if (person == null)
                return NotFound();
            Person = person;

            NaturalPerson? visitor = await personManager.GetUserAsync(User);

            Members = organizationMemberManager.GetVisibleMembersOf(person, visitor);

            if (!User.Identity!.IsAuthenticated)
            {
                return Page();
            }

            if (User.SubjectId() == Person.Id)
                UserIsOwner = true;

            return Page();
        }
    }
}
