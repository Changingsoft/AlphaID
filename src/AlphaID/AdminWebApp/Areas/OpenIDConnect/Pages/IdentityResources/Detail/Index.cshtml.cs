using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityResources.Detail;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public IdentityResource Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        IdentityResource? idr = await dbContext.IdentityResources
            .Include(p => p.UserClaims)
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (idr == null)
            return NotFound();
        Data = idr;
        return Page();
    }
}