using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public class IndexModel(ConfigurationDbContext context) : PageModel
{
    public IEnumerable<Client> Clients { get; set; } = default!;

    public void OnGet()
    {
        Clients = context.Clients
            .Include(p => p.AllowedGrantTypes)
            .Include(p => p.RedirectUris)
            .Include(p => p.AllowedScopes);
    }
}