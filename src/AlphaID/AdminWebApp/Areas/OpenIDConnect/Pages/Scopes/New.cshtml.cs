using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class NewModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public NewModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
        this.Input = new InputModel()
        {
            ShowInDiscoveryDocument = true,
        };
    }

    public async Task<IActionResult> OnPost()
    {
        var now = DateTime.UtcNow;
        var scope = new Duende.IdentityServer.EntityFramework.Entities.ApiScope()
        {
            Enabled = true,
            Name = this.Input.Name,
            DisplayName = this.Input.DisplayName,
            Description = this.Input.Description,
            Required = this.Input.Required,
            Emphasize = this.Input.Emphasize,
            ShowInDiscoveryDocument = this.Input.ShowInDiscoveryDocument, //todo 该属性已停用。
            Created = now,
            Updated = now,
        };

        if (!this.ModelState.IsValid)
            return this.Page();

        try
        {
            this.dbContext.ApiScopes.Add(scope);
            await this.dbContext.SaveChangesAsync();
            return this.RedirectToPage("Detail/Index", new { id = scope.Id });
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
        }
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; } = default!;

        [Display(Name = "Display name")]
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
