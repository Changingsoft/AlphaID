using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Components;

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
