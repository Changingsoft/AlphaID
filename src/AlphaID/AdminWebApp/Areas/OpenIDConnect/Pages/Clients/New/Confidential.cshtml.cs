using System.ComponentModel.DataAnnotations;
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
    public string ClientId { get; set; } = default!;

    [Display(Name = "Client name")]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientName { get; set; } = default!;

    [Display(Name = "Client secret")]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientSecret { get; set; } = secretGenerator.Generate();

    [Display(Name = "Sign-in callback URI")]
    [DataType(DataType.Url, ErrorMessage = "Validate_DataType_Url")]
    public string SigninCallbackUri { get; set; } = default!;

    public List<SelectListItem> ScopeItems { get; set; } = default!;

    public List<SelectListItem> AllowedGrantTypes { get; set; } =
    [
        new SelectListItem("授权码", GrantType.AuthorizationCode, true),
        new SelectListItem("客户端凭据", GrantType.ClientCredentials, false),
        new SelectListItem("资源所有者密码", GrantType.ResourceOwnerPassword, false)
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