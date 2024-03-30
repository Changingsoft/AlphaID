using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityProviders.Detail;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public IdentityProvider Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var idp = await dbContext.IdentityProviders.FindAsync(id);
        if (idp == null)
            return NotFound();

        Data = idp;
        return Page();
    }
}
