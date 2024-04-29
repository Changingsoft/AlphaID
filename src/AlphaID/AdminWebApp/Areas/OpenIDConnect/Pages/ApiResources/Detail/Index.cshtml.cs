using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail;

public class IndexModel(ConfigurationDbContext context) : PageModel
{
    public ApiResource Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        ApiResource? resource = await context.ApiResources
            .Include(p => p.Secrets)
            .Include(p => p.Scopes)
            .Include(p => p.UserClaims)
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;
        return Page();
    }
}