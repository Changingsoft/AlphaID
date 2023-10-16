using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

[IgnoreAntiforgeryToken]  //hack 由于使用InputModel类作为输入模型，导致AddionalFields标记被添加模型属性名前缀，导致AntiForgery失效。
public class CreateModel : PageModel
{
    private readonly ConfigurationDbContext context;

    public CreateModel(ConfigurationDbContext context)
    {
        this.context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
        this.Input = new InputModel();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var now = DateTime.UtcNow;
        var client = new Duende.IdentityServer.EntityFramework.Entities.Client()
        {
            Enabled = true,
            ClientId = this.Input.ClientId,
            ProtocolType = "oidc", //Default to "oidc"
            RequireClientSecret = this.Input.RequireClientSecret,
            ClientName = this.Input.ClientName,
            Description = this.Input.Description,
            AbsoluteRefreshTokenLifetime = this.Input.AbsoluteRefreshTokenLifetime,
            AccessTokenLifetime = this.Input.AccessTokenLifetime,
            AccessTokenType = this.Input.AccessTokenType,
            AllowAccessTokensViaBrowser = this.Input.AllowAccessTokensViaBrowser,
            AllowedIdentityTokenSigningAlgorithms = this.Input.AllowedIdentityTokenSigningAlgorithms,
            AllowOfflineAccess = this.Input.AllowOfflineAccess,
            AllowPlainTextPkce = this.Input.AllowPlainTextPkce,
            AllowRememberConsent = this.Input.AllowRememberConsent,
            AlwaysIncludeUserClaimsInIdToken = this.Input.AlwaysIncludeUserClaimsInIdToken,
            AlwaysSendClientClaims = this.Input.AlwaysSendClientClaims,
            AuthorizationCodeLifetime = this.Input.AuthorizationCodeLifetime,
            BackChannelLogoutSessionRequired = this.Input.BackChannelLogoutSessionRequired,
            BackChannelLogoutUri = this.Input.BackChannelLogoutUri,
            CibaLifetime = this.Input.CibaLifetime,
            ClientClaimsPrefix = this.Input.ClientClaimsPrefix,
            ClientUri = this.Input.ClientUri,
            ConsentLifetime = this.Input.ConsentLifetime,
            CoordinateLifetimeWithUserSession = this.Input.CoordinateLifetimeWithUserSession,
            DeviceCodeLifetime = this.Input.DeviceCodeLifetime,
            EnableLocalLogin = this.Input.EnableLocalLogin,
            FrontChannelLogoutSessionRequired = this.Input.FrontChannelLogoutSessionRequired,
            FrontChannelLogoutUri = this.Input.FrontChannelLogoutUri,
            IdentityTokenLifetime = this.Input.IdentityTokenLifetime,
            IncludeJwtId = this.Input.IncludeJwtId,
            LogoUri = this.Input.LogoUri,
            PairWiseSubjectSalt = this.Input.PairWiseSubjectSalt,
            PollingInterval = this.Input.PollingInterval,
            RefreshTokenExpiration = this.Input.RefreshTokenExpiration,
            RefreshTokenUsage = this.Input.RefreshTokenUsage,
            RequireConsent = this.Input.RequireConsent,
            RequirePkce = this.Input.RequirePkce,
            RequireRequestObject = this.Input.RequireRequestObject,
            SlidingRefreshTokenLifetime = this.Input.SlidingRefreshTokenLifetime,
            UpdateAccessTokenClaimsOnRefresh = this.Input.UpdateAccessTokenClaimsOnRefresh,
            UserCodeType = this.Input.UserCodeType,
            UserSsoLifetime = this.Input.UserSsoLifetime,

            Created = now,
            Updated = now,

            AllowedCorsOrigins = new List<ClientCorsOrigin>(),
            AllowedGrantTypes = new List<ClientGrantType>(),
            AllowedScopes = new List<ClientScope>(),
            Claims = new List<Duende.IdentityServer.EntityFramework.Entities.ClientClaim>(),
            ClientSecrets = new List<ClientSecret>(),
            IdentityProviderRestrictions = new List<ClientIdPRestriction>(),
            PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>(),
            Properties = new List<ClientProperty>(),
            RedirectUris = new List<ClientRedirectUri>(),
        };

        if (this.Input.RequireClientSecret)
        {
            if (string.IsNullOrWhiteSpace(this.Input.ClientSecret))
                this.ModelState.AddModelError("Input.ClientSecret", "当选择需要客户端密钥时，请提供客户端密钥。");
            else
            {
                client.ClientSecrets.Add(new ClientSecret()
                {
                    Created = now,
                    Type = "SharedSecret",
                    Value = this.Input.ClientSecret.ToSha256(),
                });
            }
        }

        if (!string.IsNullOrWhiteSpace(this.Input.RedirectUri))
        {
            client.RedirectUris.Add(new ClientRedirectUri()
            {
                RedirectUri = this.Input.RedirectUri,
            });
        }
        client.AllowedGrantTypes.Add(new ClientGrantType()
        {
            GrantType = this.Input.DefaultGrantType,
        });

        if (!this.ModelState.IsValid)
            return this.Page();

        //添加默认的Scopes
        client.AllowedScopes.Add(new ClientScope() { Scope = "openid" });
        client.AllowedScopes.Add(new ClientScope() { Scope = "profile" });
        client.AllowedScopes.Add(new ClientScope() { Scope = "user_impersonation" });

        try
        {
            this.context.Clients.Add(client);
            await this.context.SaveChangesAsync();
            return this.RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
        }

        return this.Page();
    }

    public IActionResult OnPostCheckClientIdConflict(string clientId)
    {
        return new JsonResult(!this.context.Clients.Any(p => p.ClientId == clientId));
    }

    public class InputModel
    {
        [Display(Name = "Client ID", Description="Identifier used by OAuth/OIDC protocol.")]
        [PageRemote(PageHandler = "CheckClientIdConflict", AdditionalFields = "__RequestVerificationToken", HttpMethod = "post", ErrorMessage = "Client Id already exists.")]
        public string ClientId { get; set; } = Guid.NewGuid().ToString().ToLower();

        [Display(Name = "Client name",Description ="Friendly name for display.")]
        public string ClientName { get; set; } = default!;

        [Display(Name = "Require client secret", Description ="Specifies the client is a credential client.")]
        public bool RequireClientSecret { get; set; } = true;

        [Display(Name = "Client secret", Description = "The value will be display only once here, please remember it carefully. It can reset after client created.")]
        public string? ClientSecret { get; set; }

        [Display(Name = "Redirect URI", Prompt = "https://example.com/signin-oidc")]
        public string? RedirectUri { get; set; }

        [Display(Name = "Default grant type")]
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

        [Display(Name = "Always include user claims in Id Token")]
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

        [Display(Name = "Identity Token lifetime (secends)")]
        public int IdentityTokenLifetime { get; set; } = 300;

        [Display(Name = "Allowed identity token signing algorithms")]
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }

        [Display(Name = "Access Token lifetime (secends)")]
        public int AccessTokenLifetime { get; set; } = 3600;

        [Display(Name = "Authorization code lifetime (secends)")]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        [Display(Name = "Consent lifetime")]
        public int? ConsentLifetime { get; set; } = null;

        [Display(Name = "Absolute refresh token lifetime (secends)")]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        [Display(Name = "Sliding refresh token lifetime (secends)")]
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
