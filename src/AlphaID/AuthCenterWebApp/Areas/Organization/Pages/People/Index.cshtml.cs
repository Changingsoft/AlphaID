using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class IndexModel(OrganizationMemberManager organizationMemberManager, OrganizationManager organizationManager, NaturalPersonManager personManager) : PageModel
    {
        public GenericOrganization Organization { get; set; } = default!;

        public IEnumerable<OrganizationMember> Members { get; set; } = [];

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who");
            if (organization == null)
                return this.NotFound();
            this.Organization = organization;

            var visitor = await personManager.GetUserAsync(this.User);

            this.Members = await organizationMemberManager.GetVisibleMembersAsync(this.Organization, visitor);
            return this.Page();
        }

        public async Task<IActionResult> OnPostLeaveAsync(string anchor, string personId)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who");
            if (organization == null)
                return this.NotFound();
            this.Organization = organization;

            var visitor = await personManager.GetUserAsync(this.User);

            this.Members = await organizationMemberManager.GetVisibleMembersAsync(this.Organization, visitor);

            var member = this.Members.FirstOrDefault(m => m.PersonId == personId);
            if (member == null)
                return this.Page();

            this.Result = await organizationMemberManager.LeaveOrganizationAsync(member);
            return this.Page();
        }
    }
}
