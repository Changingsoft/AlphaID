using System.ComponentModel.DataAnnotations;
using AspNetWebLib.Validations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Client = Duende.IdentityServer.EntityFramework.Entities.Client;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.New;

[BindProperties]
public class ConfidentialModel(ConfigurationDbContext dbContext, ISecretGenerator secretGenerator) : PageModel
{
    [Display(Name = "Client Id")]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientId { get; set; } = null!;

    [Display(Name = "Client name")]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Client secret")]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientSecret { get; set; } = secretGenerator.Generate();

    [Display(Name = "Sign-in callback URI", Prompt = "https://example.com/signin-oidc")]
    [CustomUrl]
    public string? SigninCallbackUri { get; set; }

    public List<SelectListItem> ScopeItems { get; set; } = null!;

    public List<SelectListItem> AllowedGrantTypes { get; set; } =
    [
        new("授权码", GrantType.AuthorizationCode, true),
        new("客户端凭据", GrantType.ClientCredentials, false),
        new("资源所有者密码", GrantType.ResourceOwnerPassword, false)
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
        if (AllowedGrantTypes.Any(s => s is { Value: GrantType.AuthorizationCode, Selected: true })
            || AllowedGrantTypes.Any(s => s is { Value: GrantType.ResourceOwnerPassword, Selected: true }))
        {
            if (!ScopeItems.Any(s => s.Selected))
                ModelState.AddModelError(nameof(ScopeItems), "Authorization code or ROPC workflow requires at least one scope(s).");
            if(SigninCallbackUri == null)
                ModelState.AddModelError(nameof(SigninCallbackUri), "Authorization code or ROPC workflow requires a sign-in callback URI.");
        }

        if (!ModelState.IsValid)
            return Page();

        DateTime now = DateTime.UtcNow;
        var client = new Client
        {
            Enabled = true,
            ClientId = ClientId,
            ProtocolType = "oidc", //Default to "oidc"
            RequireClientSecret = true,
            ClientName = ClientName,

            Created = now,
            Updated = now,

            AllowedCorsOrigins = [],
            AllowedGrantTypes = [.. AllowedGrantTypes.Where(s => s.Selected).Select(s => new ClientGrantType() { GrantType = s.Value })],
            AllowedScopes = [.. ScopeItems.Where(s => s.Selected).Select(s => new ClientScope() { Scope = s.Value })],
            Claims = [],
            ClientSecrets =
            [
                new ClientSecret()
                {
                    Type = "Shared",
                    Value = ClientSecret.ToSha256(),
                }
            ],
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