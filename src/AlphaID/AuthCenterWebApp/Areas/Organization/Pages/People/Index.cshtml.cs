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

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who");
            if (organization == null)
                return this.NotFound();
            this.Organization = organization;

            var visitor = await this.personManager.GetUserAsync(this.User);

            this.Members = await this.organizationMemberManager.GetVisibleMembersAsync(this.Organization, visitor);
            return this.Page();
        }

        public async Task<IActionResult> OnPostLeaveAsync(string anchor, string personId)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who");
            if (organization == null)
                return this.NotFound();
            this.Organization = organization;

            var visitor = await this.personManager.GetUserAsync(this.User);

            this.Members = await this.organizationMemberManager.GetVisibleMembersAsync(this.Organization, visitor);

            var member = this.Members.FirstOrDefault(m => m.PersonId == personId);
            if (member == null)
                return this.Page();

            this.Result = await this.organizationMemberManager.LeaveOrganizationAsync(member);
            return this.Page();
        }
    }
}
