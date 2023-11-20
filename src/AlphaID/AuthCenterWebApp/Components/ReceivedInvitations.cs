using IdSubjects;
using IdSubjects.Invitations;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenterWebApp.Components;

public class ReceivedInvitations : ViewComponent
{
    private readonly JoinOrganizationInvitationManager manager;

    public ReceivedInvitations(JoinOrganizationInvitationManager manager)
    {
        this.manager = manager;
    }

    public IViewComponentResult Invoke(NaturalPerson person)
    {
        var invitations = this.manager.GetPendingInvitations(person);
        return this.View(model: invitations);
    }
}
