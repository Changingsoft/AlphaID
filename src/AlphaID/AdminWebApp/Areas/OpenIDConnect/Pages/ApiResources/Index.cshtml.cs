using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public IQueryable<ApiResource> Results { get; set; } = default!;

    public void OnGet()
    {
        Results = dbContext.ApiResources;
    }
}
