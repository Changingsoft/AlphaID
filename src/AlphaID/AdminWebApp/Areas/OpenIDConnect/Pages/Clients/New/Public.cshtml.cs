using System.ComponentModel.DataAnnotations;
using AspNetWebLib.Validations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.New;

[BindProperties]
public class PublicModel(ConfigurationDbContext dbContext) : PageModel
{
    [Display(Name = "Client Id")]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientId { get; set; } = null!;

    [Display(Name = "Client name")]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Sign-in callback URI", Prompt = "https://example.com/signin-oidc")]
    [CustomUrl()]
    public string SigninCallbackUri { get; set; } = null!;

    //todo Identity resources and scope selected.

    public List<SelectListItem> ScopeItems { get; set; } = null!;

    public List<SelectListItem> AllowedGrantTypes { get; set; } =
    [
        new("授权码", GrantType.AuthorizationCode, true),
        new("隐式", GrantType.Implicit, false)
    ];

    public void OnGet()
    {
        ScopeItems = dbContext.IdentityResources.ToList().Select(s =>
                new SelectListItem(s.DisplayName, s.Name, false, !s.Enabled))
            .Union(dbContext.ApiScopes.ToList().Select(s =>
                new SelectListItem(s.DisplayName, s.Name, false, !s.Enabled)))
            .ToList();

        ClientId = Guid.NewGuid().ToString();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ScopeItems.Any(s => s.Selected))
            ModelState.AddModelError(nameof(ScopeItems), "Select at least one scope(s).");

        if (!ModelState.IsValid)
            return Page();
        DateTime now = DateTime.UtcNow;
        var client = new Duende.IdentityServer.EntityFramework.Entities.Client
        {
            Enabled = true,
            ClientId = ClientId,
            ProtocolType = "oidc", //Default to "oidc"
            RequireClientSecret = false,
            ClientName = ClientName,

            Created = now,
            Updated = now,

            AllowedCorsOrigins = [],
            AllowedGrantTypes = [.. AllowedGrantTypes.Where(s => s.Selected).Select(s => new ClientGrantType() { GrantType = s.Value })],
            AllowedScopes = [.. ScopeItems.Where(s => s.Selected).Select(s => new ClientScope() { Scope = s.Value })],
            Claims = [],
            ClientSecrets = [],
            IdentityProviderRestrictions = [],
            PostLogoutRedirectUris = [],
            Properties = [],
            RedirectUris = []
        };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        return RedirectToPage("../Detail/Index", new { anchor = client.Id });
    }
}