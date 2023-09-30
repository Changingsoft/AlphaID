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
        [Display(Name = "����")]
        public string Name { get; set; } = default!;

        [Display(Name = "��ʾ����")]
        public string? DisplayName { get; set; }

        [Display(Name = "����")]
        public string? Description { get; set; }

        [Display(Name = "�����", Description = "����")]
        public bool Required { get; set; }

        [Display(Name = "Emphasize", Description = "ǿ��")]
        public bool Emphasize { get; set; }

        [Display(Name = "�ڷ����ĵ����г�")]
        public bool ShowInDiscoveryDocument { get; set; }

    }
}
