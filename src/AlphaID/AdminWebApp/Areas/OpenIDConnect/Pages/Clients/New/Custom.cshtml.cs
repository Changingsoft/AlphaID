using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Client = Duende.IdentityServer.EntityFramework.Entities.Client;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.New;

public class CustomeModel(ConfigurationDbContext context, ISecretGenerator secretGenerator) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Client Id", Description = "Unique ID of the client.")]
    [PageRemote(PageHandler = "CheckClientIdConflict", AdditionalFields = "__RequestVerificationToken",
        HttpMethod = "post", ErrorMessage = "The client id already exists.")]
    public string ClientId { get; set; } = Guid.NewGuid().ToString().ToLower();

    public void OnGet()
    {
        Input = new InputModel
        {
            ClientSecret = secretGenerator.Generate()
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        DateTime now = DateTime.UtcNow;
        var client = new Client
        {
            Enabled = true,
            ClientId = ClientId,
            ProtocolType = "oidc", //Default to "oidc"
            RequireClientSecret = Input.RequireClientSecret,
            ClientName = Input.ClientName,
            Description = Input.Description,
            AbsoluteRefreshTokenLifetime = Input.AbsoluteRefreshTokenLifetime,
            AccessTokenLifetime = Input.AccessTokenLifetime,
            AccessTokenType = Input.AccessTokenType,
            AllowAccessTokensViaBrowser = Input.AllowAccessTokensViaBrowser,
            AllowedIdentityTokenSigningAlgorithms = Input.AllowedIdentityTokenSigningAlgorithms,
            AllowOfflineAccess = Input.AllowOfflineAccess,
            AllowPlainTextPkce = Input.AllowPlainTextPkce,
            AllowRememberConsent = Input.AllowRememberConsent,
            AlwaysIncludeUserClaimsInIdToken = Input.AlwaysIncludeUserClaimsInIdToken,
            AlwaysSendClientClaims = Input.AlwaysSendClientClaims,
            AuthorizationCodeLifetime = Input.AuthorizationCodeLifetime,
            BackChannelLogoutSessionRequired = Input.BackChannelLogoutSessionRequired,
            BackChannelLogoutUri = Input.BackChannelLogoutUri,
            CibaLifetime = Input.CibaLifetime,
            ClientClaimsPrefix = Input.ClientClaimsPrefix,
            ClientUri = Input.ClientUri,
            ConsentLifetime = Input.ConsentLifetime,
            CoordinateLifetimeWithUserSession = Input.CoordinateLifetimeWithUserSession,
            DeviceCodeLifetime = Input.DeviceCodeLifetime,
            EnableLocalLogin = Input.EnableLocalLogin,
            FrontChannelLogoutSessionRequired = Input.FrontChannelLogoutSessionRequired,
            FrontChannelLogoutUri = Input.FrontChannelLogoutUri,
            IdentityTokenLifetime = Input.IdentityTokenLifetime,
            IncludeJwtId = Input.IncludeJwtId,
            LogoUri = Input.LogoUri,
            PairWiseSubjectSalt = Input.PairWiseSubjectSalt,
            PollingInterval = Input.PollingInterval,
            RefreshTokenExpiration = Input.RefreshTokenExpiration,
            RefreshTokenUsage = Input.RefreshTokenUsage,
            RequireConsent = Input.RequireConsent,
            RequirePkce = Input.RequirePkce,
            RequireRequestObject = Input.RequireRequestObject,
            SlidingRefreshTokenLifetime = Input.SlidingRefreshTokenLifetime,
            UpdateAccessTokenClaimsOnRefresh = Input.UpdateAccessTokenClaimsOnRefresh,
            UserCodeType = Input.UserCodeType,
            UserSsoLifetime = Input.UserSsoLifetime,

            Created = now,
            Updated = now,

            AllowedCorsOrigins = [],
            AllowedGrantTypes = [],
            AllowedScopes = [],
            Claims = [],
            ClientSecrets = [],
            IdentityProviderRestrictions = [],
            PostLogoutRedirectUris = [],
            Properties = [],
            RedirectUris = []
        };

        if (Input.RequireClientSecret)
        {
            if (string.IsNullOrWhiteSpace(Input.ClientSecret))
                ModelState.AddModelError("Input.ClientSecret", "当选择需要客户端密钥时，请提供客户端密钥。");
            else
                client.ClientSecrets.Add(new ClientSecret
                {
                    Created = now,
                    Type = "SharedSecret",
                    Value = Input.ClientSecret.ToSha256()
                });
        }

        if (!string.IsNullOrWhiteSpace(Input.RedirectUri))
            client.RedirectUris.Add(new ClientRedirectUri
            {
                RedirectUri = Input.RedirectUri
            });
        client.AllowedGrantTypes.Add(new ClientGrantType
        {
            GrantType = Input.DefaultGrantType
        });

        if (!ModelState.IsValid)
            return Page();

        //添加默认的Scopes
        client.AllowedScopes.Add(new ClientScope { Scope = "openid" });
        client.AllowedScopes.Add(new ClientScope { Scope = "profile" });
        client.AllowedScopes.Add(new ClientScope { Scope = "user_impersonation" });

        try
        {
            context.Clients.Add(client);
            await context.SaveChangesAsync();
            return RedirectToPage("../Detail/Index", new { anchor = client.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        return Page();
    }

    public IActionResult OnPostCheckClientIdConflict(string clientId)
    {
        return new JsonResult(!context.Clients.Any(p => p.ClientId == clientId));
    }

    public class InputModel
    {
        [Display(Name = "Client name", Description = "Friendly name for display.")]
        [Required(ErrorMessage = "Validate_Required")]
        public string ClientName { get; set; } = null!;

        [Display(Name = "Require client secret", Description = "Specifies the client is a credential client.")]
        public bool RequireClientSecret { get; set; } = true;

        [Display(Name = "Client secret",
            Description =
                "The value will be display only once here, please remember it carefully. It can reset after client created.")]
        public string? ClientSecret { get; set; }

        [Display(Name = "Redirect URI", Prompt = "https://example.com/signin-oidc")]
        public string? RedirectUri { get; set; }

        [Display(Name = "Default grant type")]
        [Required(ErrorMessage = "Validate_Required")]
        public string DefaultGrantType { get; set; } = "authorization_code";

        [Display(Name = "Description", Description = "Description for this client.")]
        public string? Description { get; set; }

        [Display(Name = "Client URI", Prompt = "https://example.com")]
        public string? ClientUri { get; set; }

        [Display(Name = "Logo URI", Prompt = "https://example.com/logo.jpg")]
        public string? LogoUri { get; set; }

        [Display(Name = "Require consent")]
        public bool RequireConsent { get; set; } = false;

        [Display(Name = "Allow remember consent")]
        public bool AllowRememberConsent { get; set; } = true;

        [Display(Name = "Always include user claims in Anchor Token")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Display(Name = "Require PKCE")]
        public bool RequirePkce { get; set; } = true;

        [Display(Name = "Allow plain text PKCE")]
        public bool AllowPlainTextPkce { get; set; }

        [Display(Name = "Require request object")]
        public bool RequireRequestObject { get; set; }

        [Display(Name = "Allow access tokens via browser")]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Display(Name = "Front channel logout URI")]
        [DataType(DataType.Url)]
        public string? FrontChannelLogoutUri { get; set; }

        [Display(Name = "Front channel logout session required")]
        public bool FrontChannelLogoutSessionRequired { get; set; }

        [Display(Name = "Back channel logout URI")]
        [DataType(DataType.Url)]
        public string? BackChannelLogoutUri { get; set; }

        [Display(Name = "Back channel logout session required")]
        public bool BackChannelLogoutSessionRequired { get; set; }

        [Display(Name = "Allow offline Access")]
        public bool AllowOfflineAccess { get; set; }

        [Display(Name = "Identity Token lifetime (seconds)")]
        public int IdentityTokenLifetime { get; set; } = 300;

        [Display(Name = "Allowed identity token signing algorithms")]
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }

        [Display(Name = "Access Token lifetime (seconds)")]
        public int AccessTokenLifetime { get; set; } = 3600;

        [Display(Name = "Authorization code lifetime (seconds)")]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        [Display(Name = "Consent lifetime")]
        public int? ConsentLifetime { get; set; } = null;

        [Display(Name = "Absolute refresh token lifetime (seconds)")]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        [Display(Name = "Sliding refresh token lifetime (seconds)")]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        [Display(Name = "Refresh token usage (0:Reuse, 1:One time)")]
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;

        [Display(Name = "Update access token claims on refresh")]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [Display(Name = "Refresh token expiration (0:Sliding, 1:Absolute)")]
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;

        [Display(Name = "Access token type")]
        public int AccessTokenType { get; set; } = 0; // AccessTokenType.Jwt;

        [Display(Name = "Enable local login")]
        public bool EnableLocalLogin { get; set; } = true;

        [Display(Name = "Include JWT ID")]
        public bool IncludeJwtId { get; set; }

        [Display(Name = "Always send client claims")]
        public bool AlwaysSendClientClaims { get; set; }

        [Display(Name = "Client claims prefix")]
        public string? ClientClaimsPrefix { get; set; } = "client_";

        [Display(Name = "Pair wise subject salt")]
        public string? PairWiseSubjectSalt { get; set; }

        [Display(Name = "User SSO lifetime")]
        public int? UserSsoLifetime { get; set; }

        [Display(Name = "User code type")]
        public string? UserCodeType { get; set; }

        [Display(Name = "Device code lifetime")]
        public int DeviceCodeLifetime { get; set; } = 300;

        [Display(Name = "CIBA lifetime")]
        public int? CibaLifetime { get; set; }

        [Display(Name = "Polling interval")]
        public int? PollingInterval { get; set; }

        [Display(Name = "Coordinate lifetime with user session")]
        public bool? CoordinateLifetimeWithUserSession { get; set; }
    }
}