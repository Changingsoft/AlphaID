using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources;

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
        this.Input = new InputModel();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var now = DateTime.Now;
        var resource = new ApiResource
        {
            Name = this.Input.Name,
            DisplayName = this.Input.DisplayName,
            Description = this.Input.Description,
            AllowedAccessTokenSigningAlgorithms = this.Input.AllowedAccessTokenSigningAlgorithms,
            RequireResourceIndicator = this.Input.RequireResourceIndicator,
            ShowInDiscoveryDocument = this.Input.ShowInDiscoveryDocument,
            Enabled = true,
            Created = now,
            Updated = now,
        };

        try
        {
            this.dbContext.ApiResources.Add(resource);
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
        [Display(Name = "��ԴID")]
        public string Name { get; set; } = default!;

        [Display(Name = "��ʾ����")]
        public string? DisplayName { get; set; }

        [Display(Name = "����")]
        public string? Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "����ķ�������ǩ���㷨")]
        public string? AllowedAccessTokenSigningAlgorithms { get; set; }

        [Display(Name = "�ڷ����ĵ�����ʾ")]
        public bool ShowInDiscoveryDocument { get; set; } = true;

        [Display(Name = "��Ҫ��Դָʾ��")]
        public bool RequireResourceIndicator { get; set; }

    }
}
