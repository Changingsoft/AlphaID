using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail
{
    public class IdPRestrictionModel : PageModel
    {
        private readonly ConfigurationDbContext dbContext;

        public IdPRestrictionModel(ConfigurationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.IdProviders = this.dbContext.IdentityProviders.Where(p => p.Enabled).Select(p => new SelectListItem(p.DisplayName, p.Scheme));
        }

        public Client Data { get; set; } = default!;
        public IEnumerable<SelectListItem> IdProviders { get; set; }

        [BindProperty]
        [Display(Name = "Selected Provider")]
        public string SelectedProvider { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var client = this.dbContext.Clients.Include(p => p.IdentityProviderRestrictions).FirstOrDefault(c => c.Id == id);
            if (client == null)
                return this.NotFound();
            this.Data = client;
            return this.Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int itemId)
        {
            var client = this.dbContext.Clients.Include(p => p.IdentityProviderRestrictions).FirstOrDefault(c => c.Id == id);
            if (client == null)
                return this.NotFound();
            this.Data = client;
            var item = this.Data.IdentityProviderRestrictions.FirstOrDefault(p => p.Id == itemId);
            if(item != null)
            {
                this.Data.IdentityProviderRestrictions.Remove(item);
                this.dbContext.Clients.Update(this.Data);
                await this.dbContext.SaveChangesAsync();
            }
            return this.Page();
        }

        public async Task<IActionResult> OnPostAddAsync(int id)
        {
            var client = this.dbContext.Clients.Include(p => p.IdentityProviderRestrictions).FirstOrDefault(c => c.Id == id);
            if (client == null)
                return this.NotFound();
            this.Data = client;

            if (this.Data.IdentityProviderRestrictions.Any(p => p.Provider == this.SelectedProvider))
                this.ModelState.AddModelError(nameof(SelectedProvider), "选择的Id Provider已经在列表中。");

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Data.IdentityProviderRestrictions.Add(new ClientIdPRestriction
            {
                Provider = this.SelectedProvider,
            });
            this.dbContext.Clients.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
            return this.Page();

        }
    }
}
