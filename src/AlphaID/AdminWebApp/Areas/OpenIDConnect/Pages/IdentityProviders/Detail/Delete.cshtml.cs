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
        IdentityProvider? idp = await dbContext.IdentityProviders.FindAsync(id);
        if (idp == null) return NotFound();

        Data = idp;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        IdentityProvider? idp = await dbContext.IdentityProviders.FindAsync(id);
        if (idp == null) return NotFound();

        Data = idp;

        if (SchemeName != Data.Scheme)
            ModelState.AddModelError(nameof(SchemeName), "Scheme name not matched.");

        if (!ModelState.IsValid)
            return Page();

        dbContext.IdentityProviders.Remove(Data);
        await dbContext.SaveChangesAsync();
        return RedirectToPage("../Index");
    }
}