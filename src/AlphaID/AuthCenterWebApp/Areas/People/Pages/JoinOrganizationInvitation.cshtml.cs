using IdSubjects;
using IdSubjects.Invitations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.People.Pages
{
    public class JoinOrganizationInvitationModel(NaturalPersonManager personManager, JoinOrganizationInvitationManager invitationsManager) : PageModel
    {
        public NaturalPerson Person { get; set; } = default!;

        public JoinOrganizationInvitation Invitation { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor, int invitationId)
        {
            var person = await personManager.FindByNameAsync(anchor);
            if (person == null)
            {
                return this.NotFound();
            }
            this.Person = person;
            var invitations = invitationsManager.GetPendingInvitations(person);
            var invitation = invitations.FirstOrDefault(i => i.Id == invitationId);
            if (invitation == null)
            {
                return this.NotFound();
            }

            this.Invitation = invitation;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor, int invitationId, string button, MembershipVisibility visibility)
        {
            var person = await personManager.FindByNameAsync(anchor);
            if (person == null)
            {
                return this.NotFound();
            }
            this.Person = person;
            var invitations = invitationsManager.GetPendingInvitations(person);
            var invitation = invitations.FirstOrDefault(i => i.Id == invitationId);
            if (invitation == null)
            {
                return this.NotFound();
            }
            this.Invitation = invitation;

            if (button == "Accept")
            {
                invitation.ExpectVisibility = visibility;
                this.Result = await invitationsManager.AcceptAsync(invitation);
            }
            else
            {
                this.Result = await invitationsManager.RefuseAsync(invitation);
            }
            return this.Page();
        }
    }
}
