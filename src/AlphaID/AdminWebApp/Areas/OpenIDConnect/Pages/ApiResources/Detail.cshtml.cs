using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources;

public class DetailModel : PageModel
{
    private readonly ConfigurationDbContext context;

    public DetailModel(ConfigurationDbContext context)
    {
        this.context = context;
    }

    public ApiResource Data { get; set; } = default!;

    public async Task<IActionResult> OnGet(int id)
    {
        var resource = await this.context.ApiResources
            .Include(p => p.Secrets)
            .Include(p => p.Scopes)
            .Include(p => p.UserClaims)
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) { return this.NotFound(); }

        this.Data = resource;
        return this.Page();

    }
}
