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
        Client? data = dbContext.Clients.Include(p => p.AllowedScopes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        ScopeItems = dbContext.IdentityResources.ToList().Select(s =>
                new SelectListItem(s.DisplayName, s.Name, Data.AllowedScopes.Any(p => p.Scope == s.Name), !s.Enabled))
            .Union(dbContext.ApiScopes.ToList().Select(s =>
                new SelectListItem(s.DisplayName, s.Name, Data.AllowedScopes.Any(p => p.Scope == s.Name), !s.Enabled)))
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.AllowedScopes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        foreach (SelectListItem scope in ScopeItems)
            switch (scope.Selected)
            {
                case true:
                    if (!Data.AllowedScopes.Any(p => p.Scope == scope.Value))
                        Data.AllowedScopes.Add(new ClientScope
                        {
                            ClientId = Data.Id,
                            Scope = scope.Value
                        });
                    break;
                case false:
                    ClientScope? item = Data.AllowedScopes.FirstOrDefault(p => p.Scope == scope.Value);
                    if (item != null) Data.AllowedScopes.Remove(item);
                    break;
            }

        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();
        OperationMessage = "操作已成功！";


        return Page();
    }
}