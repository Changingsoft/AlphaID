using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class ScopesModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public ScopesModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Client Data { get; set; } = default!;

    [BindProperty]
    public List<SelectListItem> ScopeItems { get; set; }

    public IActionResult OnGet(int id)
    {
        var data = this.dbContext.Clients.Include(p => p.AllowedScopes).FirstOrDefault(p => p.Id == id);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        this.ScopeItems = this.dbContext.IdentityResources.ToList().Select(s => new SelectListItem(s.DisplayName, s.Name, this.Data.AllowedScopes.Any(p => p.Scope == s.Name), !s.Enabled))
            .Union(this.dbContext.ApiScopes.ToList().Select(s => new SelectListItem(s.DisplayName, s.Name, this.Data.AllowedScopes.Any(p => p.Scope == s.Name), !s.Enabled)))
            .ToList();
            
        return this.Page();
    }

    public async Task<IActionResult> OnPost(int id)
    {
        var data = this.dbContext.Clients.Include(p => p.AllowedScopes).FirstOrDefault(p => p.Id == id);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        foreach (var scope in this.ScopeItems)
        {
            switch (scope.Selected)
            {
                case true:
                    if(!this.Data.AllowedScopes.Any(p => p.Scope == scope.Value))
                    {
                        this.Data.AllowedScopes.Add(new ClientScope()
                        {
                            ClientId = this.Data.Id,
                            Scope = scope.Value,
                        });
                    }
                    break;
                case false:
                    var item = this.Data.AllowedScopes.FirstOrDefault(p => p.Scope == scope.Value);
                    if(item != null)
                    {
                        this.Data.AllowedScopes.Remove(item);
                    }
                    break;
            }
        }
        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();

        return this.Page();
    }
}
