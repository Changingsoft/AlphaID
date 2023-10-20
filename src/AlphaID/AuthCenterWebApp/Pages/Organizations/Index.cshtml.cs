using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Organizations
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly OrganizationMemberManager organizationMemberManager;
        private readonly NaturalPersonManager naturalPersonManager;

        public IndexModel(OrganizationMemberManager organizationMemberManager, NaturalPersonManager naturalPersonManager)
        {
            this.organizationMemberManager = organizationMemberManager;
            this.naturalPersonManager = naturalPersonManager;
        }

        public IEnumerable<OrganizationMember> OrganizationMembers { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await this.naturalPersonManager.GetUserAsync(this.User);
            if (person == null)
                return this.NotFound();
            this.OrganizationMembers = await this.organizationMemberManager.GetMembersOfAsync(person);
            return this.Page();
        }
    }
}
