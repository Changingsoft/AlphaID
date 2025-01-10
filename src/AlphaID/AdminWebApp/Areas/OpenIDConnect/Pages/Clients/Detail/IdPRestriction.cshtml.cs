using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class IdPRestrictionModel : PageModel
{
    private readonly ConfigurationDbContext _dbContext;

    public IdPRestrictionModel(ConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
        IdProviders = _dbContext.IdentityProviders.Where(p => p.Enabled)
            .Select(p => new SelectListItem(p.DisplayName, p.Scheme));
    }

    public Client Data { get; set; } = null!;
    public IEnumerable<SelectListItem> IdProviders { get; set; }

    [BindProperty]
    [Display(Name = "Selected Provider")]
    public string SelectedProvider { get; set; } = null!;

    public IActionResult OnGet(int anchor)
    {
        Client? client = _dbContext.Clients.Include(p => p.IdentityProviderRestrictions)
            .FirstOrDefault(c => c.Id == anchor);
        if (client == null)
            return NotFound();
        Data = client;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int anchor, int itemId)
    {
        Client? client = _dbContext.Clients.Include(p => p.IdentityProviderRestrictions)
            .FirstOrDefault(c => c.Id == anchor);
        if (client == null)
            return NotFound();
        Data = client;
        ClientIdPRestriction? item = Data.IdentityProviderRestrictions.FirstOrDefault(p => p.Id == itemId);
        if (item != null)
        {
            Data.IdentityProviderRestrictions.Remove(item);
            _dbContext.Clients.Update(Data);
            await _dbContext.SaveChangesAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int anchor)
    {
        Client? client = _dbContext.Clients.Include(p => p.IdentityProviderRestrictions)
            .FirstOrDefault(c => c.Id == anchor);
        if (client == null)
            return NotFound();
        Data = client;

        if (Data.IdentityProviderRestrictions.Any(p => p.Provider == SelectedProvider))
            ModelState.AddModelError(nameof(SelectedProvider), "选择的Id Provider已经在列表中。");

        if (!ModelState.IsValid)
            return Page();

        Data.IdentityProviderRestrictions.Add(new ClientIdPRestriction
        {
            Provider = SelectedProvider
        });
        _dbContext.Clients.Update(Data);
        await _dbContext.SaveChangesAsync();
        return Page();
    }
}