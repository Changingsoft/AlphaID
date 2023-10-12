using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class AdvancedModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public AdvancedModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Client Data { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    [BindProperty]
    public AddPropertyModel AddProperty { get; set; }

    public IActionResult OnGet(int id)
    {
        var data = this.dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == id);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        this.Input = new InputModel
        {
            AbsoluteRefreshTokenLifetime = data.AbsoluteRefreshTokenLifetime,
            AccessTokenLifetime = data.AccessTokenLifetime,
            AllowAccessTokensViaBrowser = data.AllowAccessTokensViaBrowser,
            AllowedIdentityTokenSigningAlgorithms = data.AllowedIdentityTokenSigningAlgorithms,
            AllowOfflineAccess = data.AllowOfflineAccess,
            AlwaysIncludeUserClaimsInIdToken = data.AlwaysIncludeUserClaimsInIdToken,
            AuthorizationCodeLifetime = data.AuthorizationCodeLifetime,
            BackChannelLogoutSessionRequired = data.BackChannelLogoutSessionRequired,
            BackChannelLogoutUri = data.BackChannelLogoutUri,
            CibaLifetime = data.CibaLifetime,
            ClientClaimsPrefix = data.ClientClaimsPrefix,
            ConsentLifetime = data.ConsentLifetime,
            CoordinateLifetimeWithUserSession = data.CoordinateLifetimeWithUserSession,
            DeviceCodeLifetime = data.DeviceCodeLifetime,
            DPoPClockSkew = data.DPoPClockSkew,
            DPoPValidationMode = data.DPoPValidationMode,
            EnableLocalLogin = data.EnableLocalLogin,
            FrontChannelLogoutSessionRequired = data.FrontChannelLogoutSessionRequired,
            FrontChannelLogoutUri = data.FrontChannelLogoutUri,
            IdentityTokenLifetime = data.IdentityTokenLifetime,
            IncludeJwtId = data.IncludeJwtId,
            InitiateLoginUri = data.InitiateLoginUri,
            PairWiseSubjectSalt = data.PairWiseSubjectSalt,
            PollingInterval = data.PollingInterval,
            RefreshTokenExpiration = data.RefreshTokenExpiration,
            RefreshTokenUsage = data.RefreshTokenUsage,
            RequireDPoP = data.RequireDPoP,
            RequireRequestObject = data.RequireRequestObject,
            SlidingRefreshTokenLifetime = data.SlidingRefreshTokenLifetime,
            UpdateAccessTokenClaimsOnRefresh = data.UpdateAccessTokenClaimsOnRefresh,
            UserCodeType = data.UserCodeType,
            UserSsoLifetime = data.UserSsoLifetime,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var data = this.dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == id);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        if (!this.ModelState.IsValid)
            return this.Page();

        //modify
        this.Data.AbsoluteRefreshTokenLifetime = this.Input.AbsoluteRefreshTokenLifetime;
        this.Data.AccessTokenLifetime = this.Input.AccessTokenLifetime;
        //
        this.Data.AllowAccessTokensViaBrowser = this.Input.AllowAccessTokensViaBrowser;
        this.Data.AllowedIdentityTokenSigningAlgorithms = this.Input.AllowedIdentityTokenSigningAlgorithms;
        this.Data.AllowOfflineAccess = this.Input.AllowOfflineAccess;
        this.Data.AlwaysIncludeUserClaimsInIdToken = this.Input.AlwaysIncludeUserClaimsInIdToken;
        this.Data.AuthorizationCodeLifetime = this.Input.AuthorizationCodeLifetime;
        this.Data.BackChannelLogoutSessionRequired = this.Input.BackChannelLogoutSessionRequired;
        this.Data.BackChannelLogoutUri = this.Input.BackChannelLogoutUri;
        this.Data.CibaLifetime = this.Input.CibaLifetime;
        this.Data.ClientClaimsPrefix = this.Input.ClientClaimsPrefix;
        this.Data.ConsentLifetime = this.Input.ConsentLifetime;
        this.Data.CoordinateLifetimeWithUserSession = this.Input.CoordinateLifetimeWithUserSession;
        this.Data.DeviceCodeLifetime = this.Input.DeviceCodeLifetime;
        this.Data.DPoPClockSkew = this.Input.DPoPClockSkew;
        this.Data.DPoPValidationMode = this.Input.DPoPValidationMode;
        this.Data.EnableLocalLogin = this.Input.EnableLocalLogin;
        this.Data.FrontChannelLogoutSessionRequired = this.Input.FrontChannelLogoutSessionRequired;
        this.Data.FrontChannelLogoutUri = this.Input.FrontChannelLogoutUri;
        this.Data.IdentityTokenLifetime = this.Input.IdentityTokenLifetime;
        this.Data.IncludeJwtId = this.Input.IncludeJwtId;
        this.Data.InitiateLoginUri = this.Input.InitiateLoginUri;
        this.Data.PairWiseSubjectSalt = this.Input.PairWiseSubjectSalt;
        this.Data.PollingInterval = this.Input.PollingInterval;
        this.Data.RefreshTokenExpiration = this.Input.RefreshTokenExpiration;
        this.Data.RefreshTokenUsage = this.Input.RefreshTokenUsage;
        this.Data.RequireDPoP = this.Input.RequireDPoP;
        this.Data.RequireRequestObject = this.Input.RequireRequestObject;
        this.Data.SlidingRefreshTokenLifetime = this.Input.SlidingRefreshTokenLifetime;
        this.Data.UpdateAccessTokenClaimsOnRefresh = this.Input.UpdateAccessTokenClaimsOnRefresh;
        this.Data.UserCodeType = this.Input.UserCodeType;
        this.Data.UserSsoLifetime = this.Input.UserSsoLifetime;

        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int id, int propId)
    {
        var data = this.dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == id);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        var item = this.Data.Properties.FirstOrDefault(p => p.Id == propId);
        if(item != null)
        {
            this.Data.Properties.Remove(item);
            this.dbContext.Clients.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
        }
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int id)
    {
        var data = this.dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == id);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        if(this.Data.Properties.Any(p => p.Key == this.AddProperty.Key && p.Value == this.AddProperty.Value))
        {
            this.ModelState.AddModelError("", "指定的Key和Value已存在。");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        this.Data.Properties.Add(new ClientProperty { Key = this.AddProperty.Key, Value = this.AddProperty.Value });
        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "Always Include User Claims In Id Token", Description = "When requesting both an id token and access token, should the user claims always be added to the id token instead of requiring the client to use the userinfo endpoint. Default is false.")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Display(Name = "Require Request Object", Description = "Specifies whether this client needs to wrap the authorize request parameters in a JWT (defaults to false)")]
        public bool RequireRequestObject { get; set; }

        [Display(Name = "Allow Access Tokens Via Browser", Description = "Specifies whether this client is allowed to receive access tokens via the browser. This is useful to harden flows that allow multiple response types (e.g. by disallowing a hybrid flow client that is supposed to use code id_token to add the token response type and thus leaking the token to the browser).")]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Display(Name = "Require DPoP", Description = "Specifies whether a DPoP (Demonstrating Proof-of-Possession) token is requied to be used by this client. Defaults to false.")]
        public bool RequireDPoP { get; set; }

        [Display(Name = "DPoP Validation Mode", Description = "Enum setting to control validation for the DPoP proof token expiration. This supports both the client generated ‘iat’ value and/or the server generated ‘nonce’ value. Defaults to DPoPTokenExpirationValidationMode.Iat, which only validates the ‘iat’ value.")]
        public Duende.IdentityServer.Models.DPoPTokenExpirationValidationMode DPoPValidationMode { get; set; }

        [Display(Name = "DPoP Clock Skew", Description = "Clock skew used in validating the client’s DPoP proof token ‘iat’ claim value. Defaults to 5 minutes.")]
        public TimeSpan DPoPClockSkew { get; set; } = TimeSpan.FromMinutes(5);

        [Display(Name = "Front Channel Logout URI", Description = "Specifies logout URI at client for HTTP based front-channel logout.")]
        public string? FrontChannelLogoutUri { get; set; }

        [Display(Name = "Front Channel Logout Session Required", Description = "Specifies if the user’s session id should be sent to the FrontChannelLogoutUri. Defaults to true.")]
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        [Display(Name = "Back Channel Logout URI", Description = "Specifies logout URI at client for HTTP based back-channel logout.")]
        public string? BackChannelLogoutUri { get; set; }

        [Display(Name = "Back Channel Logout Session Required", Description = "Specifies if the user’s session id should be sent in the request to the BackChannelLogoutUri. Defaults to true.")]
        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        [Display(Name = "Allow Offline Access", Description = "Specifies whether this client can request refresh tokens (be requesting the offline_access scope)")]
        public bool AllowOfflineAccess { get; set; }

        [Display(Name = "Identity Token Lifetime", Description = "Lifetime to identity token in seconds (defaults to 300 seconds / 5 minutes)")]
        public int IdentityTokenLifetime { get; set; } = 300;

        [Display(Name = "Allowed Identity Token Signing Algorithms", Description = "List of allowed signing algorithms for identity token. If empty, will use the server default signing algorithm.")]
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }

        [Display(Name = "Access Token Lifetime", Description = "Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)")]
        public int AccessTokenLifetime { get; set; } = 3600;

        [Display(Name = "Authorization Code Lifetime", Description = "Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)")]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        [Display(Name = "Consent Lifetime", Description = "Lifetime of a user consent in seconds. Defaults to null (no expiration).")]
        public int? ConsentLifetime { get; set; } = null;

        [Display(Name = "Absolute Refresh Token Lifetime", Description = "Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds / 30 days")]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        [Display(Name = "Sliding Refresh Token Lifetime", Description = "Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days")]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        [Display(Name = "Refresh Token Usage", Description = "")]
        public int RefreshTokenUsage { get; set; } = (int)Duende.IdentityServer.Models.TokenUsage.OneTimeOnly;

        [Display(Name = "Update Access Token Claims On Refresh", Description = "Gets or sets a value indicating whether the access token (and its claims) should be updated on a refresh token request.")]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [Display(Name = "Refresh Token Expiration", Description = "")]
        public int RefreshTokenExpiration { get; set; } = (int)Duende.IdentityServer.Models.TokenExpiration.Absolute;

        [Display(Name = "Enable Local Login", Description = "Specifies if this client can use local accounts, or external IdPs only. Defaults to true.")]
        public bool EnableLocalLogin { get; set; } = true;

        [Display(Name = "Include Jwt Id", Description = "Specifies whether JWT access tokens should have an embedded unique ID (via the jti claim). Defaults to true.")]
        public bool IncludeJwtId { get; set; }

        [Display(Name = "Client Claims Prefix", Description = "If set, the prefix client claim types will be prefixed with. Defaults to client_. The intent is to make sure they don’t accidentally collide with user claims.")]
        public string ClientClaimsPrefix { get; set; } = "client_";

        [Display(Name = "Pairwise Subject Salt", Description = "Salt value used in pair-wise subjectId generation for users of this client. Currently not implemented.")]
        public string? PairWiseSubjectSalt { get; set; }

        [Display(Name = "Initiate Login URI", Description = "An optional URI that can be used to initiate login from the IdentityServer host or a third party. This is most commonly used to create a client application portal within the IdentityServer host. Defaults to null.")]
        public string? InitiateLoginUri { get; set; }

        [Display(Name = "User SSO Lifetime", Description = "The maximum duration (in seconds) since the last time the user authenticated. Defaults to null. You can adjust the lifetime of a session token to control when and how often a user is required to reenter credentials instead of being silently authenticated, when using a web application.")]
        public int? UserSsoLifetime { get; set; }

        [Display(Name = "User Code Type", Description = "Specifies the type of user code to use for the client. Otherwise falls back to default.")]
        public string? UserCodeType { get; set; }

        [Display(Name = "Device Code Lifetime", Description = "Lifetime to device code in seconds (defaults to 300 seconds / 5 minutes)")]
        public int DeviceCodeLifetime { get; set; } = 300;

        [Display(Name = "CIBA Lifetime", Description = "Specifies the backchannel authentication request lifetime in seconds. Defaults to null.")]
        public int? CibaLifetime { get; set; }

        [Display(Name = "Polling Interval", Description = "Backchannel polling interval in seconds. Defaults to null.")]
        public int? PollingInterval { get; set; }

        [Display(Name = "Coordinate Lifetime With User Session", Description = "When enabled, the client’s token lifetimes (e.g. refresh tokens) will be tied to the user’s session lifetime. This means when the user logs out, any revokable tokens will be removed. If using server-side sessions, expired sessions will also remove any revokable tokens, and backchannel logout will be triggered. This client’s setting overrides the global CoordinateTokensWithUserSession configuration setting.")]
        public bool? CoordinateLifetimeWithUserSession { get; set; }

    }

    public class AddPropertyModel
    {
        [Display(Name = "Key")]
        public string Key { get; set; } = default!;

        [Display(Name = "Value")]
        public string Value { get; set; } = default!;
    }
}
