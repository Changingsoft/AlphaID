using AlphaIdPlatform.Invitations;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenterWebApp.Components;

public class ReceivedInvitations(JoinOrganizationInvitationManager manager) : ViewComponent
{
    public IViewComponentResult Invoke(string personId)
    {
        IEnumerable<JoinOrganizationInvitation> invitations = manager.GetPendingInvitations(personId);
        return View(invitations);
    }
}