using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class CorsModel(ConfigurationDbContext dbContext) : PageModel
{
    public Client Data { get; set; } = null!;

    [BindProperty]
    [Display(Name = "New origin")]
    public string NewOrigin { get; set; } = null!;

    public IActionResult OnGet(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.AllowedCorsOrigins).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int anchor, int originId)
    {
        Client? data = dbContext.Clients.Include(p => p.AllowedCorsOrigins).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        ClientCorsOrigin? item = Data.AllowedCorsOrigins.FirstOrDefault(p => p.Id == originId);
        if (item != null)
        {
            Data.AllowedCorsOrigins.Remove(item);
            dbContext.Clients.Update(Data);
            await dbContext.SaveChangesAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.AllowedCorsOrigins).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        if (Data.AllowedCorsOrigins.Any(p => p.Origin == NewOrigin))
            ModelState.AddModelError(nameof(NewOrigin), "The Origin has been existed.");

        if (!ModelState.IsValid) return Page();

        Data.AllowedCorsOrigins.Add(new ClientCorsOrigin
        {
            Origin = NewOrigin
        });
        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }
}