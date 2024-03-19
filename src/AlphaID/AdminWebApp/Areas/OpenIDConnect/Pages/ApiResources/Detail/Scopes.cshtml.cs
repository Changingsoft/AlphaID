using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class ScopesModel : PageModel
    {
        private readonly ConfigurationDbContext dbContext;

        public ScopesModel(ConfigurationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.AllScopes = this.dbContext.ApiScopes.Select(p => p.Name);
        }

        public ApiResource Data { get; set; } = default!;

        [BindProperty]
        public string SelectedScope { get; set; } = default!;

        public IEnumerable<string> AllScopes { get; set; }

        public IEnumerable<string> RemainingScopes { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.Scopes)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            this.RemainingScopes = this.AllScopes.Except(this.Data.Scopes.Select(p => p.Scope));

            return this.Page();

        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int scopeId)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.Scopes)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            var item = this.Data.Scopes.FirstOrDefault(p => p.Id == scopeId);
            if (item != null)
            {
                this.Data.Scopes.Remove(item);
                this.dbContext.ApiResources.Update(this.Data);
                await this.dbContext.SaveChangesAsync();
            }
            this.RemainingScopes = this.AllScopes.Except(this.Data.Scopes.Select(p => p.Scope));
            return this.Page();
        }

        public async Task<IActionResult> OnPostAddAsync(int id)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.Scopes)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            if (!this.ModelState.IsValid)
                return this.Page();

            this.Data.Scopes.Add(new ApiResourceScope
            {
                Scope = this.SelectedScope,
            });
            this.dbContext.ApiResources.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
            this.RemainingScopes = this.AllScopes.Except(this.Data.Scopes.Select(p => p.Scope));
            return this.Page();
        }
    }
}
