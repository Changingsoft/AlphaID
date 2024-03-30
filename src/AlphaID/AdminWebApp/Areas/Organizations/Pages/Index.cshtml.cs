using IdSubjects;

namespace AdminWebApp.Areas.Organizations.Pages
{
    public class IndexModel(IOrganizationStore organizationStore) : PageModel
    {
        public IEnumerable<GenericOrganization> Last10Orgs { get; set; } = default!;

        public int OrgCount { get; set; }

        public void OnGet()
        {
            Last10Orgs = organizationStore.Organizations.OrderByDescending(p => p.WhenChanged).Take(10);
            OrgCount = organizationStore.Organizations.Count();
        }
    }
}
