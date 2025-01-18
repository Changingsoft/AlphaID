using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Components;

public class OrganizationNavPanel(IOrganizationStore organizationStore) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(organizationStore.Organizations.Count());
    }
}