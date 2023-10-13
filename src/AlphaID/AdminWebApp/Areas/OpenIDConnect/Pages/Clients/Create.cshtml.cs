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
        var now = DateTime.Now;
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
        [Display(Name = "客户端ID", Description="Identifier used by OAuth/OIDC protocol.")]
        [PageRemote(PageHandler = "CheckClientIdConflict", AdditionalFields = "__RequestVerificationToken", HttpMethod = "post", ErrorMessage = "Client Id already exists.")]
        public string ClientId { get; set; } = Guid.NewGuid().ToString().ToLower();

        [Display(Name = "名称",Description ="Friendly name for display.")]
        public string ClientName { get; set; } = default!;

        [Display(Name = "需要客户端密钥",Description ="Specifies the client is a credential client.")]
        public bool RequireClientSecret { get; set; } = true;

        [Display(Name = "客户端密钥", Description = "The value will be display only once here, please remember it carefully. It can reset after client created.")]
        public string? ClientSecret { get; set; }

        [Display(Name = "回调URI", Prompt = "例如：https://example.com/signin-oidc")]
        public string? RedirectUri { get; set; }

        [Display(Name = "默认授权类型（模式）")]
        public string DefaultGrantType { get; set; } = "authorization_code";

        [Display(Name = "描述", Description = "Description for this client.")]
        public string? Description { get; set; }

        [Display(Name = "客户端URI", Prompt = "例如：https://example.com")]
        public string? ClientUri { get; set; }

        [Display(Name = "Logo URI", Prompt = "例如：https://example.com/logo.jpg")]
        public string? LogoUri { get; set; }

        [Display(Name = "需要用户同意")]
        public bool RequireConsent { get; set; } = false;

        [Display(Name = "允许记住同意")]
        public bool AllowRememberConsent { get; set; } = true;

        [Display(Name = "在Id Token中始终包含用户声明")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Display(Name = "需要PKCE")]
        public bool RequirePkce { get; set; } = true;

        [Display(Name = "允许明文PKCE")]
        public bool AllowPlainTextPkce { get; set; }

        [Display(Name = "需要请求对象")]
        public bool RequireRequestObject { get; set; }

        [Display(Name = "Allow Access Tokens Via Browser")]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Display(Name = "Front Channel Logout Uri")]
        [DataType(DataType.Url)]
        public string? FrontChannelLogoutUri { get; set; }

        [Display(Name = "Front Channel Logout Session Required")]
        public bool FrontChannelLogoutSessionRequired { get; set; }

        [Display(Name = "Back Channel Logout Uri")]
        [DataType(DataType.Url)]
        public string? BackChannelLogoutUri { get; set; }

        [Display(Name = "Back Channel Logout Session Required")]
        public bool BackChannelLogoutSessionRequired { get; set; }

        [Display(Name = "允许离线访问")]
        public bool AllowOfflineAccess { get; set; }

        [Display(Name = "Identity Token的生存期（秒）")]
        public int IdentityTokenLifetime { get; set; } = 300;

        [Display(Name = "允许的Identity Token签名算法")]
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }

        [Display(Name = "Access Token生存期（秒）")]
        public int AccessTokenLifetime { get; set; } = 3600;

        [Display(Name = "授权码生存期（秒）")]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        [Display(Name = "ConsentLifetime")]
        public int? ConsentLifetime { get; set; } = null;

        [Display(Name = "绝对刷新令牌生存期（秒）")]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        [Display(Name = "滑动刷新令牌生存期（秒）")]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        [Display(Name = "刷新令牌用法（0：可重用，1：一次性）")]
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;

        [Display(Name = "刷新时更新Access Token中的声明")]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [Display(Name = "刷新令牌时效（0：滑动，1：绝对）")]
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;

        [Display(Name = "Access Token类型")]
        public int AccessTokenType { get; set; } = 0; // AccessTokenType.Jwt;

        [Display(Name = "启用本地登录")]
        public bool EnableLocalLogin { get; set; } = true;

        [Display(Name = "包括JWT ID")]
        public bool IncludeJwtId { get; set; }

        [Display(Name = "始终发送客户端声明")]
        public bool AlwaysSendClientClaims { get; set; }

        [Display(Name = "客户端声明前缀")]
        public string? ClientClaimsPrefix { get; set; } = "client_";

        [Display(Name = "PairWiseSubjectSalt")]
        public string? PairWiseSubjectSalt { get; set; }

        [Display(Name = "UserSsoLifetime")]
        public int? UserSsoLifetime { get; set; }

        [Display(Name = "User Code Type")]
        public string? UserCodeType { get; set; }

        [Display(Name = "Device Code Lifetime")]
        public int DeviceCodeLifetime { get; set; } = 300;

        [Display(Name = "Ciba Lifetime")]
        public int? CibaLifetime { get; set; }

        [Display(Name = "Polling Interval")]
        public int? PollingInterval { get; set; }

        [Display(Name = "Coordinate Lifetime With User Session")]
        public bool? CoordinateLifetimeWithUserSession { get; set; }
    }
}
