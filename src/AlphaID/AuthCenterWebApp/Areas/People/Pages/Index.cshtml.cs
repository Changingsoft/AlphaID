using AlphaIDPlatform.Security;
using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace AuthCenterWebApp.Areas.People.Pages
{
    public class IndexModel : PageModel
    {
        private readonly NaturalPersonManager personManager;
        private readonly OrganizationMemberManager organizationMemberManager;


        public IndexModel(NaturalPersonManager personManager, OrganizationMemberManager organizationMemberManager)
        {
            this.personManager = personManager;
            this.organizationMemberManager = organizationMemberManager;
        }

        public NaturalPerson Person { get; set; } = default!;

        public bool UserIsOwner { get; set; } = false;

        public IEnumerable<OrganizationMember> Members { get; set; } = Enumerable.Empty<OrganizationMember>();

        public async Task<IActionResult> OnGetAsync(string userAnchor)
        {
            //Support both userAnchor and user ID.
            var person = await this.personManager.FindByNameAsync(userAnchor)
                ?? await this.personManager.FindByIdAsync(userAnchor);
            if (person == null)
                return this.NotFound();
            this.Person = person;

            this.Members = await this.organizationMemberManager.GetMembersOfAsync(person);

            if(this.User.Identity!.IsAuthenticated)
            {
                if (this.User.SubjectId() == this.Person.Id)
                    this.UserIsOwner = true;
            }

            return this.Page();
        }
    }
}
