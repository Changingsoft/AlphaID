using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Invitations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenterWebApp.Components;

public class ReceivedInvitations(JoinOrganizationInvitationManager manager) : ViewComponent
{
    public IViewComponentResult Invoke(NaturalPerson person)
    {
        IEnumerable<JoinOrganizationInvitation> invitations = manager.GetPendingInvitations(person);
        return View(invitations);
    }
}