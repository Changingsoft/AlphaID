using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityProviders;

public class DetailModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public DetailModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IdentityProvider Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var idp = await this.dbContext.IdentityProviders.FindAsync(id);
        if (idp == null)
            return this.NotFound();

        this.Data = idp;
        return this.Page();
    }
}
