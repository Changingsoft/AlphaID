using IdSubjects;

namespace AdminWebApp.Areas.Organizations.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IOrganizationStore organizationStore;

        public IndexModel(IOrganizationStore organizationStore)
        {
            this.organizationStore = organizationStore;
        }

        public IEnumerable<GenericOrganization> Last10Orgs { get; set; } = default!;

        public int OrgCount { get; set; }

        public void OnGet()
        {
            this.Last10Orgs = this.organizationStore.Organizations.OrderByDescending(p => p.WhenChanged).Take(10);
            this.OrgCount = this.organizationStore.Organizations.Count();
        }
    }
}
