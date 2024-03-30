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
            return NotFound();
        }
        Data = result;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var result = dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null)
        {
            return NotFound();
        }
        Data = result;

        if (ScopeName != Data.Name)
            ModelState.AddModelError(nameof(ScopeName), "���ƴ���");


        var referenced = dbContext.Clients.Any(p => p.AllowedScopes.Any(s => s.Scope == Data.Name))
            || dbContext.ApiResources.Any(p => p.Scopes.Any(s => s.Scope == Data.Name));
        if (referenced)
        {
            ModelState.AddModelError("", "���пͻ��˻�API��Դ���ø÷�Χ������ɾ���÷�Χ��");
        }


        if (!ModelState.IsValid)
            return Page();

        dbContext.ApiScopes.Remove(Data);
        await dbContext.SaveChangesAsync();
        return RedirectToPage("../Index");
    }
}
