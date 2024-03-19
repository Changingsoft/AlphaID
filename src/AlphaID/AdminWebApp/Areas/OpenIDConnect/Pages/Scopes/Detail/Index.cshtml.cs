using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes.Detail;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public ApiScope Data { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        var result = dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null)
        {
            return this.NotFound();
        }
        this.Data = result;
        return this.Page();
    }
}
