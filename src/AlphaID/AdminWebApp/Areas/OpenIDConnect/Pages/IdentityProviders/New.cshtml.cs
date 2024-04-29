using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.IdentityProviders;

public class NewModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
        Input = new InputModel();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Dictionary<string, string> properties = [];
        if (Input.Authority != null)
            properties.Add("Authority", Input.Authority);
        properties.Add("ResponseType", Input.ResponseType);
        if (Input.ClientId != null)
            properties.Add("ClientId", Input.ClientId);
        if (Input.ClientSecret != null)
            properties.Add("ClientSecret", Input.ClientSecret);
        properties.Add("Scope", Input.Scope);
        properties.Add("GetClaimsFromUserInfoEndpoint", Input.GetClaimsFromUserInfoEndpoint.ToString());
        properties.Add("UsePkce", Input.UsePkce.ToString());

        if (!ModelState.IsValid)
            return Page();

        var idp = new IdentityProvider
        {
            Scheme = Input.Scheme,
            DisplayName = Input.DisplayName,
            Enabled = true,
            Type = "oidc", //����oidc��ָʾ��ΪOidcProvider.
            Properties = JsonSerializer.Serialize(properties),
            Created = DateTime.UtcNow,
            Updated = null,
            LastAccessed = null,
            NonEditable = false
        };

        dbContext.IdentityProviders.Add(idp);
        await dbContext.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    public class InputModel
    {
        [Display(Name = "Scheme", Description = "A unique name to identity this provider.")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string Scheme { get; set; } = default!;

        [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string DisplayName { get; set; } = default!;

        /// <summary>
        /// </summary>
        [Display(Name = "Authority", Description = "The base address of the OIDC provider.")]
        public string? Authority { get; set; }

        [Display(Name = "Response type", Description = "The response type. Defaults to \"id_token\".")]
        public string ResponseType { get; set; } = "id_token";

        [Display(Name = "Client Id", Description = "The client id.")]
        public string? ClientId { get; set; }

        [Display(Name = "Client secret",
            Description =
                "The client secret. By default this is the plaintext client secret and great consideration should be taken if this value is to be stored as plaintext in the store.")]
        public string? ClientSecret { get; set; }

        [Display(Name = "Scope", Description = "Space separated list of scope values.")]
        public string Scope { get; set; } = "openid";

        [Display(Name = "Get claims from user info endpoint",
            Description = "Indicates if userinfo endpoint is to be contacted. Defaults to true.")]
        public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;

        [Display(Name = "Use PKCE", Description = "Indicates if PKCE should be used. Defaults to true.")]
        public bool UsePkce { get; set; } = true;
    }
}