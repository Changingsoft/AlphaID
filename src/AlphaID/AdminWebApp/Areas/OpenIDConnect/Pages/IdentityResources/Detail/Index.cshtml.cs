using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityResources.Detail;

public class IndexModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public IndexModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IdentityResource Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var idr = await this.dbContext.IdentityResources
            .Include(p => p.UserClaims)
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (idr == null)
            return this.NotFound();
        this.Data = idr;
        return this.Page();
    }
}
