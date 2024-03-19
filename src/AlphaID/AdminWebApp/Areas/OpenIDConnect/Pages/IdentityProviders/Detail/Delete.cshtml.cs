using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityProviders.Detail;

public class DeleteModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    public string SchemeName { get; set; } = default!;

    public IdentityProvider Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var idp = await dbContext.IdentityProviders.FindAsync(id);
        if (idp == null)
        {
            return this.NotFound();
        }

        this.Data = idp;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var idp = await dbContext.IdentityProviders.FindAsync(id);
        if (idp == null)
        {
            return this.NotFound();
        }

        this.Data = idp;

        if (this.SchemeName != this.Data.Scheme)
            this.ModelState.AddModelError(nameof(this.SchemeName), "Scheme name not matched.");

        if (!this.ModelState.IsValid)
            return this.Page();

        dbContext.IdentityProviders.Remove(this.Data);
        await dbContext.SaveChangesAsync();
        return this.RedirectToPage("../Index");
    }
}
