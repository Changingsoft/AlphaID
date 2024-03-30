using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class ScopesModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public ScopesModel(ConfigurationDbContext dbContext)
        {
            this._dbContext = dbContext;
            AllScopes = this._dbContext.ApiScopes.Select(p => p.Name);
        }

        public ApiResource Data { get; set; } = default!;

        [BindProperty]
        public string SelectedScope { get; set; } = default!;

        public IEnumerable<string> AllScopes { get; set; }

        public IEnumerable<string> RemainingScopes { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var resource = await _dbContext.ApiResources
                .Include(p => p.Scopes)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return NotFound(); }

            Data = resource;
            RemainingScopes = AllScopes.Except(Data.Scopes.Select(p => p.Scope));

            return Page();

        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int scopeId)
        {
            var resource = await _dbContext.ApiResources
                .Include(p => p.Scopes)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return NotFound(); }

            Data = resource;
            var item = Data.Scopes.FirstOrDefault(p => p.Id == scopeId);
            if (item != null)
            {
                Data.Scopes.Remove(item);
                _dbContext.ApiResources.Update(Data);
                await _dbContext.SaveChangesAsync();
            }
            RemainingScopes = AllScopes.Except(Data.Scopes.Select(p => p.Scope));
            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync(int id)
        {
            var resource = await _dbContext.ApiResources
                .Include(p => p.Scopes)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return NotFound(); }

            Data = resource;
            if (!ModelState.IsValid)
                return Page();

            Data.Scopes.Add(new ApiResourceScope
            {
                Scope = SelectedScope,
            });
            _dbContext.ApiResources.Update(Data);
            await _dbContext.SaveChangesAsync();
            RemainingScopes = AllScopes.Except(Data.Scopes.Select(p => p.Scope));
            return Page();
        }
    }
}
