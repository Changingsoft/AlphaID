using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class DetailModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public DetailModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public ApiScope Data { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        var result = this.dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null)
        {
            return this.NotFound();
        }
        this.Data = result;
        return this.Page();
    }
}
