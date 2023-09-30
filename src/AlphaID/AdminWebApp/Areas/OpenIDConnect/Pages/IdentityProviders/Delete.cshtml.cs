using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityProviders;

public class DeleteModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public DeleteModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [BindProperty]
    public string SchemeName { get; set; } = default!;

    public IdentityProvider Data { get; set; } = default!;

    public async Task<IActionResult> OnGet(int id)
    {
        var idp = await this.dbContext.IdentityProviders.FindAsync(id);
        if (idp == null)
        {
            return this.NotFound();
        }

        this.Data = idp;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var idp = await this.dbContext.IdentityProviders.FindAsync(id);
        if (idp == null)
        {
            return this.NotFound();
        }

        this.Data = idp;

        if (this.SchemeName != this.Data.Scheme)
            this.ModelState.AddModelError(nameof(this.SchemeName), "Scheme name not matched.");

        if (!this.ModelState.IsValid)
            return this.Page();

        this.dbContext.IdentityProviders.Remove(this.Data);
        await this.dbContext.SaveChangesAsync();
        return this.RedirectToPage("Index");
    }
}
