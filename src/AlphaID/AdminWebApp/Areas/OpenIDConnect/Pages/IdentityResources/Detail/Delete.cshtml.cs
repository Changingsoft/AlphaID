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
                return NotFound();
            Data = idResource;
            if (Data.NonEditable)
                return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var idResource = dbContext.IdentityResources.FirstOrDefault(p => p.Id == id);
            if (idResource == null)
                return NotFound();
            Data = idResource;
            if (Data.NonEditable)
                return NotFound();

            if (Name != Data.Name)
                ModelState.AddModelError(nameof(Name), "Invalid name.");

            if (!ModelState.IsValid)
                return Page();

            dbContext.IdentityResources.Remove(Data);
            await dbContext.SaveChangesAsync();
            return RedirectToPage("../Index");
        }
    }
}
