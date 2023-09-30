using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityProviders;

public class IndexModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public IndexModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<IdentityProvider> Results { get; set; } = default!;

    public void OnGet()
    {
        this.Results = this.dbContext.IdentityProviders;
    }
}
