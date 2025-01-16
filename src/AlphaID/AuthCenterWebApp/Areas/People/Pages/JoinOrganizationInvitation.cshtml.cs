using IdSubjects;
using IdSubjects.Invitations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.People.Pages;

public class JoinOrganizationInvitationModel(
    ApplicationUserManager<ApplicationUser> personManager,
    JoinOrganizationInvitationManager invitationsManager) : PageModel
{
    public ApplicationUser Person { get; set; } = null!;

    public JoinOrganizationInvitation Invitation { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor, int invitationId)
    {
        ApplicationUser? person = await personManager.FindByNameAsync(anchor);
        if (person == null) return NotFound();
        Person = person;
        IEnumerable<JoinOrganizationInvitation> invitations = invitationsManager.GetPendingInvitations(person);
        JoinOrganizationInvitation? invitation = invitations.FirstOrDefault(i => i.Id == invitationId);
        if (invitation == null) return NotFound();

        Invitation = invitation;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor,
        int invitationId,
        string button,
        MembershipVisibility visibility)
    {
        ApplicationUser? person = await personManager.FindByNameAsync(anchor);
        if (person == null) return NotFound();
        Person = person;
        IEnumerable<JoinOrganizationInvitation> invitations = invitationsManager.GetPendingInvitations(person);
        JoinOrganizationInvitation? invitation = invitations.FirstOrDefault(i => i.Id == invitationId);
        if (invitation == null) return NotFound();
        Invitation = invitation;

        if (button == "Accept")
        {
            invitation.ExpectVisibility = visibility;
            Result = await invitationsManager.AcceptAsync(invitation);
        }
        else
        {
            Result = await invitationsManager.RefuseAsync(invitation);
        }

        return Page();
    }
}