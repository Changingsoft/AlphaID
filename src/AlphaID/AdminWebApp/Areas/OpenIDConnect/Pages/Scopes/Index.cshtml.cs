using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class IndexModel : PageModel
{
    private readonly ConfigurationDbContext context;

    public IndexModel(ConfigurationDbContext context)
    {
        this.context = context;
    }

    public IEnumerable<ApiScope> Results { get; set; } = default!;

    public IActionResult OnGet()
    {
        this.Results = this.context.ApiScopes.OrderByDescending(p => p.Updated);
        return this.Page();
    }
}
