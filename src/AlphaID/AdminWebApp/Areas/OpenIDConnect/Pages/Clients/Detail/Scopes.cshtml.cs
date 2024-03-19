using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class ScopesModel(ConfigurationDbContext dbContext) : PageModel
{
    public Client Data { get; set; } = default!;

    [BindProperty]
    public List<SelectListItem> ScopeItems { get; set; } = default!;

    public string? OperationMessage { get; set; }

    public IActionResult OnGet(int anchor)
    {
        var data = dbContext.Clients.Include(p => p.AllowedScopes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        this.ScopeItems = dbContext.IdentityResources.ToList().Select(s => new SelectListItem(s.DisplayName, s.Name, this.Data.AllowedScopes.Any(p => p.Scope == s.Name), !s.Enabled))
            .Union(dbContext.ApiScopes.ToList().Select(s => new SelectListItem(s.DisplayName, s.Name, this.Data.AllowedScopes.Any(p => p.Scope == s.Name), !s.Enabled)))
            .ToList();

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        var data = dbContext.Clients.Include(p => p.AllowedScopes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        foreach (var scope in this.ScopeItems)
        {
            switch (scope.Selected)
            {
                case true:
                    if (!this.Data.AllowedScopes.Any(p => p.Scope == scope.Value))
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
                    if (item != null)
                    {
                        this.Data.AllowedScopes.Remove(item);
                    }
                    break;
            }
        }
        dbContext.Clients.Update(this.Data);
        await dbContext.SaveChangesAsync();
        this.OperationMessage = "操作已成功！";


        return this.Page();
    }
}
