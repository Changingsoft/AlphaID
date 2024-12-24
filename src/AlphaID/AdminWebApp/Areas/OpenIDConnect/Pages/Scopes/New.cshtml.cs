using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class NewModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
        Input = new InputModel
        {
            ShowInDiscoveryDocument = true
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        DateTime now = DateTime.UtcNow;
        var scope = new ApiScope
        {
            Enabled = true,
            Name = Input.Name,
            DisplayName = Input.DisplayName,
            Description = Input.Description,
            Required = Input.Required,
            Emphasize = Input.Emphasize,
            ShowInDiscoveryDocument = Input.ShowInDiscoveryDocument,
            Created = now,
            Updated = now
        };

        if (!ModelState.IsValid)
            return Page();

        try
        {
            dbContext.ApiScopes.Add(scope);
            await dbContext.SaveChangesAsync();
            return RedirectToPage("Detail/Index", new { id = scope.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; } = default!;

        [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
        public string? DisplayName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Required", Description = "Required.")]
        public bool Required { get; set; }

        [Display(Name = "Emphasize", Description = "Emphasize.")]
        public bool Emphasize { get; set; }

        [Display(Name = "Show in discovery document")]
        public bool ShowInDiscoveryDocument { get; set; }
    }
}