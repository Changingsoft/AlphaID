using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes.Detail;

public class DeleteModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    [Display(Name = "Scope name")]
    public string ScopeName { get; set; } = default!;

    public ApiScope Data { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        var result = dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null)
        {
            return this.NotFound();
        }
        this.Data = result;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var result = dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null)
        {
            return this.NotFound();
        }
        this.Data = result;

        if (this.ScopeName != this.Data.Name)
            this.ModelState.AddModelError(nameof(this.ScopeName), "���ƴ���");


        var referenced = dbContext.Clients.Any(p => p.AllowedScopes.Any(s => s.Scope == this.Data.Name))
            || dbContext.ApiResources.Any(p => p.Scopes.Any(s => s.Scope == this.Data.Name));
        if (referenced)
        {
            this.ModelState.AddModelError("", "���пͻ��˻�API��Դ���ø÷�Χ������ɾ���÷�Χ��");
        }


        if (!this.ModelState.IsValid)
            return this.Page();

        dbContext.ApiScopes.Remove(this.Data);
        await dbContext.SaveChangesAsync();
        return this.RedirectToPage("../Index");
    }
}
