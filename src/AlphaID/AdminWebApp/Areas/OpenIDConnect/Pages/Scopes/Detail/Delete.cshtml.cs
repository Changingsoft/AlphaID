using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes.Detail;

public class DeleteModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public DeleteModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [BindProperty]
    [Display(Name = "Scope name")]
    public string ScopeName { get; set; } = default!;

    public ApiScope Data { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        var result = this.dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null)
        {
            return this.NotFound();
        }
        this.Data = result;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var result = this.dbContext.ApiScopes.SingleOrDefault(p => p.Id == id);
        if (result == null)
        {
            return this.NotFound();
        }
        this.Data = result;

        if (this.ScopeName != this.Data.Name)
            this.ModelState.AddModelError(nameof(this.ScopeName), "名称错误。");


        var referenced = this.dbContext.Clients.Any(p => p.AllowedScopes.Any(s => s.Scope == this.Data.Name))
            || this.dbContext.ApiResources.Any(p => p.Scopes.Any(s => s.Scope == this.Data.Name));
        if (referenced)
        {
            this.ModelState.AddModelError("", "仍有客户端或API资源引用该范围，不能删除该范围。");
        }


        if (!this.ModelState.IsValid)
            return this.Page();

        this.dbContext.ApiScopes.Remove(this.Data);
        await this.dbContext.SaveChangesAsync();
        return this.RedirectToPage("../Index");
    }
}
