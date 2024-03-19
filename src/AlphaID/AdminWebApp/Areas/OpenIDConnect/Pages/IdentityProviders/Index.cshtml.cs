using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityProviders;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public IEnumerable<IdentityProvider> Results { get; set; } = default!;

    public void OnGet()
    {
        this.Results = dbContext.IdentityProviders;
    }
}
