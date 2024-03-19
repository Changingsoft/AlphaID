using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail;

public class DeleteModel(ConfigurationDbContext dbContext) : PageModel
{
    public ApiResource Data { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Resources name")]
    public string ResourceName { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var data = await dbContext.ApiResources.FindAsync(id);
        if (data == null)
            return this.NotFound();

        if (data.NonEditable)
            return this.NotFound();

        this.Data = data;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var data = await dbContext.ApiResources.FindAsync(id);
        if (data == null)
            return this.NotFound();

        this.Data = data;

        if (this.ResourceName != this.Data.DisplayName)
        {
            this.ModelState.AddModelError(nameof(this.ResourceName), "Ãû³Æ²»Æ¥Åä¡£");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        dbContext.ApiResources.Remove(this.Data);
        await dbContext.SaveChangesAsync();
        return this.RedirectToPage("../Index");
    }
}
