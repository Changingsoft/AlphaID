using Microsoft.EntityFrameworkCore;
using Organizational;

namespace AdminWebApp.Areas.OrganizationManagement.Pages;

public class IndexModel(IOrganizationStore organizationStore) : PageModel
{
    public IEnumerable<OrganizationModel> Last10Orgs { get; set; } = [];

    public int OrgCount { get; set; }

    public void OnGet()
    {
        Last10Orgs = (from org in organizationStore.Organizations.AsNoTracking()
                      orderby org.WhenCreated descending
                      select new OrganizationModel
                      {
                          Id = org.Id,
                          Name = org.Name,
                          Domicile = org.Domicile,
                          Contact = org.Contact,
                          Representative = org.Representative,
                          UpdatedAt = org.WhenChanged,
                      }).Take(10);

        OrgCount = organizationStore.Organizations.Count();
    }

    public class OrganizationModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Domicile { get; set; }

        public string? Contact { get; set; }
        public string? Representative { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}