using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Components;

public class OrganizationNavPanel : ViewComponent
{
    private readonly IOrganizationStore organizationStore;

    public OrganizationNavPanel(IOrganizationStore organizationStore)
    {
        this.organizationStore = organizationStore;
    }

    public IViewComponentResult Invoke()
    {
        return this.View(model: this.organizationStore.Organizations.Count());
    }
}
