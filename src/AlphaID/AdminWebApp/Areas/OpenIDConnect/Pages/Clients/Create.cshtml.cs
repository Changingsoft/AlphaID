using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

[IgnoreAntiforgeryToken]  //hack ����ʹ��InputModel����Ϊ����ģ�ͣ�����AddionalFields��Ǳ����ģ��������ǰ׺������AntiForgeryʧЧ��
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
                this.ModelState.AddModelError("Input.ClientSecret", "��ѡ����Ҫ�ͻ�����Կʱ�����ṩ�ͻ�����Կ��");
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

        //���Ĭ�ϵ�Scopes
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
        [Display(Name = "�ͻ���ID", Description="Identifier used by OAuth/OIDC protocol.")]
        [PageRemote(PageHandler = "CheckClientIdConflict", AdditionalFields = "__RequestVerificationToken", HttpMethod = "post", ErrorMessage = "Client Id already exists.")]
        public string ClientId { get; set; } = Guid.NewGuid().ToString().ToLower();

        [Display(Name = "����",Description ="Friendly name for display.")]
        public string ClientName { get; set; } = default!;

        [Display(Name = "��Ҫ�ͻ�����Կ",Description ="Specifies the client is a credential client.")]
        public bool RequireClientSecret { get; set; } = true;

        [Display(Name = "�ͻ�����Կ", Description = "The value will be display only once here, please remember it carefully. It can reset after client created.")]
        public string? ClientSecret { get; set; }

        [Display(Name = "�ص�URI", Prompt = "���磺https://example.com/signin-oidc")]
        public string? RedirectUri { get; set; }

        [Display(Name = "Ĭ����Ȩ���ͣ�ģʽ��")]
        public string DefaultGrantType { get; set; } = "authorization_code";

        [Display(Name = "����", Description = "Description for this client.")]
        public string? Description { get; set; }

        [Display(Name = "�ͻ���URI", Prompt = "���磺https://example.com")]
        public string? ClientUri { get; set; }

        [Display(Name = "Logo URI", Prompt = "���磺https://example.com/logo.jpg")]
        public string? LogoUri { get; set; }

        [Display(Name = "��Ҫ�û�ͬ��")]
        public bool RequireConsent { get; set; } = false;

        [Display(Name = "�����סͬ��")]
        public bool AllowRememberConsent { get; set; } = true;

        [Display(Name = "��Id Token��ʼ�հ����û�����")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Display(Name = "��ҪPKCE")]
        public bool RequirePkce { get; set; } = true;

        [Display(Name = "��������PKCE")]
        public bool AllowPlainTextPkce { get; set; }

        [Display(Name = "��Ҫ�������")]
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

        [Display(Name = "�������߷���")]
        public bool AllowOfflineAccess { get; set; }

        [Display(Name = "Identity Token�������ڣ��룩")]
        public int IdentityTokenLifetime { get; set; } = 300;

        [Display(Name = "�����Identity Tokenǩ���㷨")]
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }

        [Display(Name = "Access Token�����ڣ��룩")]
        public int AccessTokenLifetime { get; set; } = 3600;

        [Display(Name = "��Ȩ�������ڣ��룩")]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        [Display(Name = "ConsentLifetime")]
        public int? ConsentLifetime { get; set; } = null;

        [Display(Name = "����ˢ�����������ڣ��룩")]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        [Display(Name = "����ˢ�����������ڣ��룩")]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        [Display(Name = "ˢ�������÷���0�������ã�1��һ���ԣ�")]
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;

        [Display(Name = "ˢ��ʱ����Access Token�е�����")]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [Display(Name = "ˢ������ʱЧ��0��������1�����ԣ�")]
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;

        [Display(Name = "Access Token����")]
        public int AccessTokenType { get; set; } = 0; // AccessTokenType.Jwt;

        [Display(Name = "���ñ��ص�¼")]
        public bool EnableLocalLogin { get; set; } = true;

        [Display(Name = "����JWT ID")]
        public bool IncludeJwtId { get; set; }

        [Display(Name = "ʼ�շ��Ϳͻ�������")]
        public bool AlwaysSendClientClaims { get; set; }

        [Display(Name = "�ͻ�������ǰ׺")]
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
