using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class SecretsModel(ConfigurationDbContext dbContext) : PageModel
    {
        public ApiResource Data { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var resource = await dbContext.ApiResources
                .Include(p => p.Secrets)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return NotFound(); }

            Data = resource;
            return Page();

        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int secretId)
        {
            var resource = await dbContext.ApiResources
                .Include(p => p.Secrets)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return NotFound(); }

            Data = resource;
            var item = Data.Secrets.FirstOrDefault(p => p.Id == secretId);
            if (item != null)
            {
                Data.Secrets.Remove(item);
                dbContext.ApiResources.Update(Data);
                await dbContext.SaveChangesAsync();
            }
            return Page();
        }
    }
}
