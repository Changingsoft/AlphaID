using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class IndexModel(ConfigurationDbContext context) : PageModel
{
    public IEnumerable<ApiScope> Results { get; set; } = default!;

    public IActionResult OnGet()
    {
        Results = context.ApiScopes.OrderByDescending(p => p.Updated);
        return Page();
    }
}
