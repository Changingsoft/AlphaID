using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class SecretsModel : PageModel
    {
        private readonly ConfigurationDbContext dbContext;

        public SecretsModel(ConfigurationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ApiResource Data { get; set; } = default!;
        public async Task<IActionResult> OnGet(int id)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.Secrets)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            return this.Page();

        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int secretId)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.Secrets)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            var item = this.Data.Secrets.FirstOrDefault(p => p.Id == secretId);
            if(item != null)
            {
                this.Data.Secrets.Remove(item);
                this.dbContext.ApiResources.Update(this.Data);
                await this.dbContext.SaveChangesAsync();
            }
            return this.Page();
        }
    }
}
