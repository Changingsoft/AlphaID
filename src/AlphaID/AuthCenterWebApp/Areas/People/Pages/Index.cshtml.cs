using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Security;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Areas.People.Pages;

public class IndexModel(UserManager<NaturalPerson> personManager, OrganizationMemberManager organizationMemberManager)
    : PageModel
{
    public PersonModel Person { get; set; } = null!;

    public bool UserIsOwner { get; set; }

    public IEnumerable<UserMembership> Members { get; set; } = [];

    public IActionResult OnGet(string anchor)
    {
        //Support both userAnchor and user ID.
        PersonModel? person = (from user in personManager.Users.AsNoTracking()
                               where user.Id == anchor || user.UserName == anchor
                               select new PersonModel()
                               {
                                   Id = user.Id,
                                   Name = user.Name,
                                   UserName = user.UserName,
                                   Bio = user.Bio
                               }).FirstOrDefault();
        if (person == null)
            return NotFound();
        Person = person;

        Members = from member in organizationMemberManager.GetVisibleMembersOf(person.Id, User.SubjectId())
                  select member;

        if (!User.Identity!.IsAuthenticated) return Page();

        if (User.SubjectId() == Person.Id)
            UserIsOwner = true;

        return Page();
    }

    public class PersonModel
    {
        public string Id { get; set; } = null!;

        public string? Name { get; set; }

        public string? UserName { get; set; }

        public string? Bio { get; set; }
    }

    public class MemberModel
    {
        public string OrganizationName { get; set; } = null!;

        public string? Title { get; set; }

        public string? Department { get; set; }
    }
}