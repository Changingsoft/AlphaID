using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class ReferencedApiResources : ViewComponent
{
    private readonly ConfigurationDbContext dbContext;

    public ReferencedApiResources(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IViewComponentResult Invoke(string scope)
    {
        var apiResources = this.dbContext.ApiResources.Include(p => p.Scopes).Where(p => p.Scopes.Any(s => s.Scope == scope));
        return this.View(model: apiResources);
    }
}
