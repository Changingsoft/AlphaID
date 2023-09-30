using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public class IndexModel : PageModel
{
    private readonly ConfigurationDbContext context;

    public IndexModel(ConfigurationDbContext context)
    {
        this.context = context;
    }

    public IEnumerable<Client> Clients { get; set; } = default!;

    public void OnGet()
    {
        this.Clients = this.context.Clients
            .Include(p => p.AllowedGrantTypes)
            .Include(p => p.RedirectUris)
            .Include(p => p.AllowedScopes);
    }
}
