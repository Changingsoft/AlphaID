using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class IndexModel : PageModel
    {
        private readonly OrganizationMemberManager organizationMemberManager;
        private readonly OrganizationManager organizationManager;
        private readonly NaturalPersonManager personManager;

        public IndexModel(OrganizationMemberManager organizationMemberManager, OrganizationManager organizationManager, NaturalPersonManager personManager)
        {
            this.organizationMemberManager = organizationMemberManager;
            this.organizationManager = organizationManager;
            this.personManager = personManager;
        }

        public GenericOrganization Organization { get; set; } = default!;

        public IEnumerable<OrganizationMember> Members { get; set; } = Enumerable.Empty<OrganizationMember>();

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var orgs = await this.organizationManager.SearchByNameAsync(anchor);
            if (orgs.Count() > 1)
                return this.BadRequest("ÖØÃû");
            GenericOrganization? org;
            if (orgs.Any())
                org = orgs.Single();
            else
                org = await this.organizationManager.FindByIdAsync(anchor);
            if (org == null)
                return this.NotFound();

            this.Organization = org;

            var visitor = await this.personManager.GetUserAsync(this.User);

            this.Members = await this.organizationMemberManager.GetVisibleMembersAsync(this.Organization, visitor);
            return this.Page();
        }
    }
}
