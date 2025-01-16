using IdSubjects;

namespace AdminWebApp.Areas.OrganizationManagement.Pages;

public class IndexModel(IOrganizationStore organizationStore) : PageModel
{
    public IEnumerable<Organization> Last10Orgs { get; set; } = null!;

    public int OrgCount { get; set; }

    public void OnGet()
    {
        Last10Orgs = organizationStore.Organizations.OrderByDescending(p => p.WhenChanged).Take(10);
        OrgCount = organizationStore.Organizations.Count();
    }
}