using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail;

public class SecretsModel(ConfigurationDbContext dbContext) : PageModel
{
    public ApiResource Data { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.Secrets)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int id, int secretId)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.Secrets)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;
        ApiResourceSecret? item = Data.Secrets.FirstOrDefault(p => p.Id == secretId);
        if (item != null)
        {
            Data.Secrets.Remove(item);
            dbContext.ApiResources.Update(Data);
            await dbContext.SaveChangesAsync();
        }

        return Page();
    }
}