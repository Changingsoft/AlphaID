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
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            return this.Page();

        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int secretId)
        {
            var resource = await dbContext.ApiResources
                .Include(p => p.Secrets)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            var item = this.Data.Secrets.FirstOrDefault(p => p.Id == secretId);
            if (item != null)
            {
                this.Data.Secrets.Remove(item);
                dbContext.ApiResources.Update(this.Data);
                await dbContext.SaveChangesAsync();
            }
            return this.Page();
        }
    }
}
