using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public class IndexModel(ConfigurationDbContext context) : PageModel
{
    public IEnumerable<ClientModel> Clients { get; set; } = null!;

    public void OnGet()
    {
        Clients = from client in context.Clients.AsNoTracking()
                .Include(p => p.AllowedGrantTypes)
                .Include(p => p.AllowedScopes)
                  select new ClientModel()
                  {
                      Id = client.Id,
                      ClientId = client.ClientId,
                      ClientName = client.ClientName,
                      RequireClientSecret = client.RequireClientSecret,
                      RequirePkce = client.RequirePkce,
                      AllowedGrantTypes = client.AllowedGrantTypes.Select(p => p.GrantType),
                      AllowedScopes = client.AllowedScopes.Select(p => p.Scope),
                  };
    }

    public class ClientModel
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = null!;

        public string ClientName { get; set; } = null!;

        public bool RequireClientSecret { get; set; }

        public bool RequirePkce { get; set; }

        public IEnumerable<string> AllowedGrantTypes { get; set; } = null!;

        public IEnumerable<string> AllowedScopes { get; set; } = null!;
    }
}