using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class IndexModel : PageModel
    {
        private readonly OrganizationMemberManager organizationMemberManager;
        private readonly OrganizationManager organizationManager;
        private readonly IQueryableUserStore<NaturalPerson> naturalPersonStore;

        public IndexModel(OrganizationMemberManager organizationMemberManager, OrganizationManager organizationManager, IQueryableUserStore<NaturalPerson> naturalPersonStore)
        {
            this.organizationMemberManager = organizationMemberManager;
            this.organizationManager = organizationManager;
            this.naturalPersonStore = naturalPersonStore;
        }

        public GenericOrganization Organization { get; set; } = default!;

        public IEnumerable<OrganizationMember> Members { get; set; } = default!;

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
            this.Members = await this.organizationMemberManager.GetMembersAsync(this.Organization);
            return this.Page();
        }
    }
}
