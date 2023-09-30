using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityResources;

public class IndexModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public IndexModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<IdentityResource> Results { get; set; } = default!;

    public void OnGet()
    {
        this.Results = this.dbContext.IdentityResources;
    }
}
