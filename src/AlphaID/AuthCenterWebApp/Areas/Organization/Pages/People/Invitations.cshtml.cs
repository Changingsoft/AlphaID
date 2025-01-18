using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class InvitationsModel(JoinOrganizationInvitationManager manager, OrganizationManager organizationManager) : PageModel
    {
        public IEnumerable<JoinOrganizationInvitation> Invitations { get; set; } = [];

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
                return RedirectToPage("../Who", new { anchor });
            if (organization == null)
                return NotFound();
            Invitations = manager.GetIssuedInvitations(organization.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostRevoke(string anchor, int invitationId)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
                return RedirectToPage("../Who", new { anchor });
            if (organization == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            JoinOrganizationInvitation? invitation = await manager.FindById(invitationId);
            if(invitation == null)
                return NotFound();

            Result = await manager.Revoke(invitation);
            Invitations = manager.GetIssuedInvitations(organization.Id);
            return Page();
        }
    }
}
