using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class AdvancedModel(ConfigurationDbContext dbContext) : PageModel
    {
        public ApiResource Data { get; set; } = default!;

        [BindProperty]
        [Display(Name = "New key")]
        public string NewKey { get; set; } = default!;

        [BindProperty]
        [Display(Name = "New value")]
        public string NewValue { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var resource = await dbContext.ApiResources
                .Include(p => p.Properties)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            return this.Page();

        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int propId)
        {
            var resource = await dbContext.ApiResources
                .Include(p => p.Properties)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;

            var item = this.Data.Properties.FirstOrDefault(p => p.Id == propId);
            if (item != null)
            {
                this.Data.Properties.Remove(item);
                dbContext.ApiResources.Update(this.Data);
                await dbContext.SaveChangesAsync();
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAddAsync(int id)
        {
            var resource = await dbContext.ApiResources
                .Include(p => p.Properties)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;

            if (this.Data.Properties.Any(p => p.Key == this.NewKey))
                this.ModelState.AddModelError("", "The key is exists.");

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Data.Properties.Add(new ApiResourceProperty()
            {
                Key = this.NewKey,
                Value = this.NewValue,
            });
            dbContext.ApiResources.Update(this.Data);
            await dbContext.SaveChangesAsync();

            return this.Page();
        }
    }
}
