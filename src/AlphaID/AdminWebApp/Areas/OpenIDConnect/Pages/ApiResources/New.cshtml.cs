using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources;

public class NewModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public void OnGet()
    {
        Input = new InputModel();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        DateTime now = DateTime.UtcNow;
        var resource = new ApiResource
        {
            Name = Input.Name,
            DisplayName = Input.DisplayName,
            Description = Input.Description,
            AllowedAccessTokenSigningAlgorithms = Input.AllowedAccessTokenSigningAlgorithms,
            RequireResourceIndicator = Input.RequireResourceIndicator,
            Enabled = true,
            Created = now,
            Updated = now
        };

        try
        {
            dbContext.ApiResources.Add(resource);
            await dbContext.SaveChangesAsync();
            return RedirectToPage("Detail/Index", new { id = resource.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Resource ID", Description = "Unique identifier for resource in OIDC workflow.")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Name { get; set; } = null!;

        [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
        public string? DisplayName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Allowed access token signing algorithms")]
        public string? AllowedAccessTokenSigningAlgorithms { get; set; }

        [Display(Name = "Require resource indicator",
            Description =
                "Client must specify Resource ID in authorization request, so that the Audience claim that issued the token includes the Resource ID.")]
        public bool RequireResourceIndicator { get; set; }
    }
}