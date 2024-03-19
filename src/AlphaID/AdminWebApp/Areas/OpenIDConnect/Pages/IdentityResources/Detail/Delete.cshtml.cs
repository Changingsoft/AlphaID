using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityResources.Detail
{
    public class DeleteModel(ConfigurationDbContext dbContext) : PageModel
    {
        public IdentityResource Data { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Name")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string Name { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var idResource = dbContext.IdentityResources.FirstOrDefault(p => p.Id == id);
            if (idResource == null)
                return this.NotFound();
            this.Data = idResource;
            if (this.Data.NonEditable)
                return this.NotFound();
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var idResource = dbContext.IdentityResources.FirstOrDefault(p => p.Id == id);
            if (idResource == null)
                return this.NotFound();
            this.Data = idResource;
            if (this.Data.NonEditable)
                return this.NotFound();

            if (this.Name != this.Data.Name)
                this.ModelState.AddModelError(nameof(this.Name), "Invalid name.");

            if (!this.ModelState.IsValid)
                return this.Page();

            dbContext.IdentityResources.Remove(this.Data);
            await dbContext.SaveChangesAsync();
            return this.RedirectToPage("../Index");
        }
    }
}
