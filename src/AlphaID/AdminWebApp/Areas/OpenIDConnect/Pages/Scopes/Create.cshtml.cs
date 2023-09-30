using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Scopes;

public class CreateModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public CreateModel(ConfigurationDbContext dbContext)
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
        var now = DateTime.Now;
        var scope = new Duende.IdentityServer.EntityFramework.Entities.ApiScope()
        {
            Enabled = true,
            Name = this.Input.Name,
            DisplayName = this.Input.DisplayName,
            Description = this.Input.Description,
            Required = this.Input.Required,
            Emphasize = this.Input.Emphasize,
            ShowInDiscoveryDocument = this.Input.ShowInDiscoveryDocument,
            Created = now,
            Updated = now,
        };

        if (!this.ModelState.IsValid)
            return this.Page();

        try
        {
            this.dbContext.ApiScopes.Add(scope);
            await this.dbContext.SaveChangesAsync();
            return this.RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
        }
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "名称")]
        public string Name { get; set; } = default!;

        [Display(Name = "显示名称")]
        public string? DisplayName { get; set; }

        [Display(Name = "描述")]
        public string? Description { get; set; }

        [Display(Name = "必需的", Description = "必需")]
        public bool Required { get; set; }

        [Display(Name = "Emphasize", Description = "强调")]
        public bool Emphasize { get; set; }

        [Display(Name = "在发现文档中列出")]
        public bool ShowInDiscoveryDocument { get; set; }

    }
}
