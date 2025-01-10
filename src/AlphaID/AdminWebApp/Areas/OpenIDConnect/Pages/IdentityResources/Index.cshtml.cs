using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityResources;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public IEnumerable<IdentityResource> Results { get; set; } = null!;

    public void OnGet()
    {
        Results = dbContext.IdentityResources;
    }
}