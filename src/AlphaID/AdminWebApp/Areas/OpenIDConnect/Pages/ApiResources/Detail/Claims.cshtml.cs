using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class ClaimsModel : PageModel
    {
        private readonly ConfigurationDbContext dbContext;

        public ClaimsModel(ConfigurationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        [Display(Name = "New claim type")]
        public string NewClaim { get; set; } = default!;

        public ApiResource Data { get; set; } = default!;
        public async Task<IActionResult> OnGet(int id)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.UserClaims)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            return this.Page();

        }

        public async Task<IActionResult> OnPostAddAsync(int id)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.UserClaims)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            if (this.Data.UserClaims.Any(p => p.Type == this.NewClaim))
                this.ModelState.AddModelError(nameof(this.NewClaim), "ָ�������������Ѵ���");

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Data.UserClaims.Add(new ApiResourceClaim
            {
                Type = this.NewClaim,
            });
            this.dbContext.ApiResources.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
            return this.Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id, int claimId)
        {
            var resource = await this.dbContext.ApiResources
                .Include(p => p.UserClaims)
                .AsSingleQuery()
                .SingleOrDefaultAsync(p => p.Id == id);
            if (resource == null) { return this.NotFound(); }

            this.Data = resource;
            var item = this.Data.UserClaims.FirstOrDefault(p => p.Id == claimId);
            if (item != null)
            {
                this.Data.UserClaims.Remove(item);
                this.dbContext.ApiResources.Update(this.Data);
                await this.dbContext.SaveChangesAsync();
            }
            return this.Page();
        }
    }
}
