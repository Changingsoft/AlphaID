using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes.Detail;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public ApiScope Data { get; set; } = null!;

    public IActionResult OnGet(int id)
    {
        ApiScope? result = dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null) return NotFound();
        Data = result;
        return Page();
    }
}