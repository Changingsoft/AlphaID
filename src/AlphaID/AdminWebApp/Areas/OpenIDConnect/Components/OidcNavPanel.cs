using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Components;

public class OidcNavPanel(ConfigurationDbContext dbContext) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(new OidcStatistics(dbContext.Clients.Count(), dbContext.ApiScopes.Count()));
    }

    public record OidcStatistics(int ClientCount, int ScopeCount);
}