using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public class IndexModel(ConfigurationDbContext context) : PageModel
{
    public IEnumerable<ClientModel> Clients { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Keywords", Prompt = "Client ID or name")]
    [StringLength(50, ErrorMessage = "Validate_StringLength")]
    public string? Keywords { get; set; }

    public int ClientCount { get; set; }

    public void OnGet()
    {
        IQueryable<Client> clientSet = context.Clients.AsNoTracking()
            .Include(p => p.AllowedGrantTypes)
            .Include(p => p.AllowedScopes);

        if (!string.IsNullOrWhiteSpace(Keywords))
        {
            clientSet = clientSet.Where(p => p.ClientId.Contains(Keywords) || p.ClientName.Contains(Keywords));
        }

        ClientCount = clientSet.Count();
        Clients = from client in clientSet
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