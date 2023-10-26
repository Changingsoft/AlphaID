using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class CorsModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public CorsModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Client Data { get; set; } = default!;

    [BindProperty]
    [Display(Name = "New origin")]
    public string NewOrigin { get; set; } = default!;

    public IActionResult OnGet(int anchor)
    {
        var data = this.dbContext.Clients.Include(p => p.AllowedCorsOrigins).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int anchor, int originId)
    {
        var data = this.dbContext.Clients.Include(p => p.AllowedCorsOrigins).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        var item = this.Data.AllowedCorsOrigins.FirstOrDefault(p => p.Id == originId);
        if (item != null)
        {
            this.Data.AllowedCorsOrigins.Remove(item);
            this.dbContext.Clients.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
        }
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int anchor)
    {
        var data = this.dbContext.Clients.Include(p => p.AllowedCorsOrigins).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        if (this.Data.AllowedCorsOrigins.Any(p => p.Origin == this.NewOrigin))
            this.ModelState.AddModelError(nameof(this.NewOrigin), "The Origin has been existed.");

        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        this.Data.AllowedCorsOrigins.Add(new ClientCorsOrigin()
        {
            Origin = this.NewOrigin,
        });
        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();
        return this.Page();
    }
}
