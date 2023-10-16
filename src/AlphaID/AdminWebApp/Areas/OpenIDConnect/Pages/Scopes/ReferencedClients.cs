using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class ReferencedClients : ViewComponent
{
    private readonly ConfigurationDbContext dbContext;

    public ReferencedClients(ConfigurationDbContext dbCOntext)
    {
        this.dbContext = dbCOntext;
    }

    public IViewComponentResult Invoke(string scope)
    {
        var clients = this.dbContext.Clients.Include(p => p.AllowedScopes).Where(p => p.AllowedScopes.Any(p => p.Scope == scope));
        return this.View(model: clients);
    }
}
