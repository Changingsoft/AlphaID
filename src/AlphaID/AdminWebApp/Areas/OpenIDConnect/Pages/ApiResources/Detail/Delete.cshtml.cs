using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail;

public class DeleteModel(ConfigurationDbContext dbContext) : PageModel
{
    public ApiResource Data { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Resources name")]
    public string ResourceName { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        ApiResource? data = await dbContext.ApiResources.FindAsync(id);
        if (data == null)
            return NotFound();

        if (data.NonEditable)
            return NotFound();

        Data = data;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ApiResource? data = await dbContext.ApiResources.FindAsync(id);
        if (data == null)
            return NotFound();

        Data = data;

        if (ResourceName != Data.DisplayName) ModelState.AddModelError(nameof(ResourceName), "Ãû³Æ²»Æ¥Åä¡£");

        if (!ModelState.IsValid)
            return Page();

        dbContext.ApiResources.Remove(Data);
        await dbContext.SaveChangesAsync();
        return RedirectToPage("../Index");
    }
}