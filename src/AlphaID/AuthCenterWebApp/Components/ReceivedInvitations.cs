using IdSubjects;
using IdSubjects.Invitations;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenterWebApp.Components;

public class ReceivedInvitations(JoinOrganizationInvitationManager manager) : ViewComponent
{
    public IViewComponentResult Invoke(NaturalPerson person)
    {
        var invitations = manager.GetPendingInvitations(person);
        return this.View(model: invitations);
    }
}
