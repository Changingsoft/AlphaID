using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class SecretsModel(ConfigurationDbContext dbContext) : PageModel
{
    public Client Data { get; set; } = null!;

    public IActionResult OnGet(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == anchor);
        if (data == null) return NotFound();
        Data = data;
        return Page();
    }
}