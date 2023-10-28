using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Components;

public class OrganizationNavPanel : ViewComponent
{
    private readonly IQueryableOrganizationStore organizationStore;

    public OrganizationNavPanel(IQueryableOrganizationStore organizationStore)
    {
        this.organizationStore = organizationStore;
    }

    public IViewComponentResult Invoke()
    {
        return this.View(model: this.organizationStore.Organizations.Count());
    }
}
