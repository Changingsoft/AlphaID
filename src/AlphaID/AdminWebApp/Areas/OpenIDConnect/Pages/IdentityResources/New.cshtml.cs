using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityResources;

public class NewModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public List<SelectListItem> AvailableClaims { get; set; } =
    [
        new SelectListItem("sub", "sub"),
        new SelectListItem("name", "name"),
        new SelectListItem("given_name", "given_name"),
        new SelectListItem("family_name", "family_name"),
        new SelectListItem("middle_name", "middle_name"),
        new SelectListItem("nickname", "nickname"),
        new SelectListItem("preferred_username", "preferred_username"),
        new SelectListItem("profile", "profile"),
        new SelectListItem("picture", "picture"),
        new SelectListItem("website", "website"),
        new SelectListItem("email", "email"),
        new SelectListItem("email_verified", "email_verified"),
        new SelectListItem("gender", "gender"),
        new SelectListItem("birthdate", "birthdate"),
        new SelectListItem("zoneinfo", "zoneinfo"),
        new SelectListItem("locale", "locale"),
        new SelectListItem("phone_number", "phone_number"),
        new SelectListItem("phone_number_verified", "phone_number_verified"),
        new SelectListItem("address", "address"),
        new SelectListItem("updated_at", "updated_at")
    ];

    public void OnGet()
    {
        Input = new InputModel();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (dbContext.IdentityResources.Any(p => p.Name == Input.Name))
            ModelState.AddModelError("Name", "Name is exists.");

        if (!ModelState.IsValid)
            return Page();

        var identityResource = new IdentityResource
        {
            Enabled = true,
            Name = Input.Name,
            DisplayName = Input.DisplayName,
            Description = Input.Description,
            Required = Input.Required,
            Emphasize = Input.Emphasize,
            ShowInDiscoveryDocument = Input.ShowInDiscoveryDocument,
            UserClaims = null,
            Properties = null,
            Created = DateTime.Now,
            Updated = null,
            NonEditable = false
        };
        dbContext.IdentityResources.Add(identityResource);
        await dbContext.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    public record InputModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string Name { get; set; } = default!;

        [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string? DisplayName { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Validate_StringLength")]
        public string? Description { get; set; }

        [Display(Name = "Required")]
        public bool Required { get; set; } = false;

        [Display(Name = "Emphasize")]
        public bool Emphasize { get; set; } = false;

        [Display(Name = "Show in discovery document")]
        public bool ShowInDiscoveryDocument { get; set; } = true;

        public string[] SelectedClaims { get; set; } = [];
    }
}