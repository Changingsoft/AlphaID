using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

public class OrganizationNavPanel(OrganizationManager organizationStore) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View(model: organizationStore.Organizations.Count());
    }
}
