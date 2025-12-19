using AlphaIdPlatform.Invitations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class InvitationsModel(JoinOrganizationInvitationManager manager, OrganizationManager organizationManager) : PageModel
    {
        public IEnumerable<JoinOrganizationInvitation> Invitations { get; set; } = [];

        public OrganizationOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var organization = await organizationManager.FindByNameAsync(anchor);
            if (organization == null)
                return NotFound();
            Invitations = manager.GetIssuedInvitations(organization.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostRevoke(string anchor, int invitationId)
        {
            var organization = await organizationManager.FindByNameAsync(anchor);
            if (organization == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            JoinOrganizationInvitation? invitation = await manager.FindById(invitationId);
            if (invitation == null)
                return NotFound();

            Result = await manager.Revoke(invitation);
            Invitations = manager.GetIssuedInvitations(organization.Id);
            return Page();
        }
    }
}
