using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail;

public class AdvancedModel(ConfigurationDbContext dbContext) : PageModel
{
    public ApiResource Data { get; set; } = null!;

    [BindProperty]
    [Display(Name = "New key")]
    public string NewKey { get; set; } = null!;

    [BindProperty]
    [Display(Name = "New value")]
    public string NewValue { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int id, int propId)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;

        ApiResourceProperty? item = Data.Properties.FirstOrDefault(p => p.Id == propId);
        if (item != null)
        {
            Data.Properties.Remove(item);
            dbContext.ApiResources.Update(Data);
            await dbContext.SaveChangesAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int id)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;

        if (Data.Properties.Any(p => p.Key == NewKey))
            ModelState.AddModelError("", "The key is exists.");

        if (!ModelState.IsValid)
            return Page();

        Data.Properties.Add(new ApiResourceProperty
        {
            Key = NewKey,
            Value = NewValue
        });
        dbContext.ApiResources.Update(Data);
        await dbContext.SaveChangesAsync();

        return Page();
    }
}