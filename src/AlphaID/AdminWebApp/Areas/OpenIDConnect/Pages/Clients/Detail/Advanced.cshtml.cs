using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Client = Duende.IdentityServer.EntityFramework.Entities.Client;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class AdvancedModel(ConfigurationDbContext dbContext) : PageModel
{
    public Client Data { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty]
    public AddPropertyModel AddProperty { get; set; } = null!;

    public IActionResult OnGet(int anchor)
    {
        Client? data = dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        Input = new InputModel
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
            UserSsoLifetime = data.UserSsoLifetime
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        Client? data = dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        if (!ModelState.IsValid)
            return Page();

        //modify
        Data.AbsoluteRefreshTokenLifetime = Input.AbsoluteRefreshTokenLifetime;
        Data.AccessTokenLifetime = Input.AccessTokenLifetime;
        //
        Data.AllowAccessTokensViaBrowser = Input.AllowAccessTokensViaBrowser;
        Data.AllowedIdentityTokenSigningAlgorithms = Input.AllowedIdentityTokenSigningAlgorithms;
        Data.AllowOfflineAccess = Input.AllowOfflineAccess;
        Data.AlwaysIncludeUserClaimsInIdToken = Input.AlwaysIncludeUserClaimsInIdToken;
        Data.AuthorizationCodeLifetime = Input.AuthorizationCodeLifetime;
        Data.BackChannelLogoutSessionRequired = Input.BackChannelLogoutSessionRequired;
        Data.BackChannelLogoutUri = Input.BackChannelLogoutUri;
        Data.CibaLifetime = Input.CibaLifetime;
        Data.ClientClaimsPrefix = Input.ClientClaimsPrefix;
        Data.ConsentLifetime = Input.ConsentLifetime;
        Data.CoordinateLifetimeWithUserSession = Input.CoordinateLifetimeWithUserSession;
        Data.DeviceCodeLifetime = Input.DeviceCodeLifetime;
        Data.DPoPClockSkew = Input.DPoPClockSkew;
        Data.DPoPValidationMode = Input.DPoPValidationMode;
        Data.EnableLocalLogin = Input.EnableLocalLogin;
        Data.FrontChannelLogoutSessionRequired = Input.FrontChannelLogoutSessionRequired;
        Data.FrontChannelLogoutUri = Input.FrontChannelLogoutUri;
        Data.IdentityTokenLifetime = Input.IdentityTokenLifetime;
        Data.IncludeJwtId = Input.IncludeJwtId;
        Data.InitiateLoginUri = Input.InitiateLoginUri;
        Data.PairWiseSubjectSalt = Input.PairWiseSubjectSalt;
        Data.PollingInterval = Input.PollingInterval;
        Data.RefreshTokenExpiration = Input.RefreshTokenExpiration;
        Data.RefreshTokenUsage = Input.RefreshTokenUsage;
        Data.RequireDPoP = Input.RequireDPoP;
        Data.RequireRequestObject = Input.RequireRequestObject;
        Data.SlidingRefreshTokenLifetime = Input.SlidingRefreshTokenLifetime;
        Data.UpdateAccessTokenClaimsOnRefresh = Input.UpdateAccessTokenClaimsOnRefresh;
        Data.UserCodeType = Input.UserCodeType;
        Data.UserSsoLifetime = Input.UserSsoLifetime;

        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int anchor, int propId)
    {
        Client? data = dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        ClientProperty? item = Data.Properties.FirstOrDefault(p => p.Id == propId);
        if (item != null)
        {
            Data.Properties.Remove(item);
            dbContext.Clients.Update(Data);
            await dbContext.SaveChangesAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int anchor)
    {
        Client? data = dbContext.Clients
            .Include(p => p.Properties)
            .FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        if (Data.Properties.Any(p => p.Key == AddProperty.Key)) ModelState.AddModelError("", "指定的Key和Value已存在。");

        if (!ModelState.IsValid)
            return Page();

        Data.Properties.Add(new ClientProperty { Key = AddProperty.Key, Value = AddProperty.Value });
        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Always Include User Claims In Anchor Token",
            Description =
                "When requesting both an anchor token and access token, should the user claims always be added to the anchor token instead of requiring the client to use the user info endpoint. Default is false.")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Display(Name = "Require Request Object",
            Description =
                "Specifies whether this client needs to wrap the authorize request parameters in a JWT (defaults to false)")]
        public bool RequireRequestObject { get; set; }

        [Display(Name = "Allow Access Tokens Via Browser",
            Description =
                "Specifies whether this client is allowed to receive access tokens via the browser. This is useful to harden flows that allow multiple response types (e.g. by disallowing a hybrid flow client that is supposed to use code id_token to add the token response type and thus leaking the token to the browser).")]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Display(Name = "Require DPoP",
            Description =
                "Specifies whether a DPoP (Demonstrating Proof-of-Possession) token is required to be used by this client. Defaults to false.")]
        public bool RequireDPoP { get; set; }

        [Display(Name = "DPoP Validation Mode",
            Description =
                "Enum setting to control validation for the DPoP proof token expiration. This supports both the client generated ‘iat’ value and/or the server generated ‘nonce’ value. Defaults to DPoPTokenExpirationValidationMode.Iat, which only validates the ‘iat’ value.")]
        public DPoPTokenExpirationValidationMode DPoPValidationMode { get; set; }

        [Display(Name = "DPoP Clock Skew",
            Description =
                "Clock skew used in validating the client’s DPoP proof token ‘iat’ claim value. Defaults to 5 minutes.")]
        public TimeSpan DPoPClockSkew { get; set; } = TimeSpan.FromMinutes(5);

        [Display(Name = "Front Channel Logout URI",
            Description = "Specifies logout URI at client for HTTP based front-channel logout.")]
        public string? FrontChannelLogoutUri { get; set; }

        [Display(Name = "Front Channel Logout Session Required",
            Description =
                "Specifies if the user’s session anchor should be sent to the FrontChannelLogoutUri. Defaults to true.")]
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        [Display(Name = "Back Channel Logout URI",
            Description = "Specifies logout URI at client for HTTP based back-channel logout.")]
        public string? BackChannelLogoutUri { get; set; }

        [Display(Name = "Back Channel Logout Session Required",
            Description =
                "Specifies if the user’s session anchor should be sent in the request to the BackChannelLogoutUri. Defaults to true.")]
        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        [Display(Name = "Allow Offline Access",
            Description =
                "Specifies whether this client can request refresh tokens (be requesting the offline_access scope)")]
        public bool AllowOfflineAccess { get; set; }

        [Display(Name = "Identity Token Lifetime",
            Description = "Lifetime to identity token in seconds (defaults to 300 seconds / 5 minutes)")]
        public int IdentityTokenLifetime { get; set; } = 300;

        [Display(Name = "Allowed Identity Token Signing Algorithms",
            Description =
                "List of allowed signing algorithms for identity token. If empty, will use the server default signing algorithm.")]
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }

        [Display(Name = "Access Token Lifetime",
            Description = "Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)")]
        public int AccessTokenLifetime { get; set; } = 3600;

        [Display(Name = "Authorization Code Lifetime",
            Description = "Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)")]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        [Display(Name = "Consent Lifetime",
            Description = "Lifetime of a user consent in seconds. Defaults to null (no expiration).")]
        public int? ConsentLifetime { get; set; }

        [Display(Name = "Absolute Refresh Token Lifetime",
            Description = "Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds / 30 days")]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        [Display(Name = "Sliding Refresh Token Lifetime",
            Description = "Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days")]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        [Display(Name = "Refresh Token Usage", Description = "")]
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;

        [Display(Name = "Update Access Token Claims On Refresh",
            Description =
                "Gets or sets a value indicating whether the access token (and its claims) should be updated on a refresh token request.")]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [Display(Name = "Refresh Token Expiration", Description = "")]
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;

        [Display(Name = "Enable Local Login",
            Description = "Specifies if this client can use local accounts, or external IdPs only. Defaults to true.")]
        public bool EnableLocalLogin { get; set; } = true;

        [Display(Name = "Include Jwt Anchor",
            Description =
                "Specifies whether JWT access tokens should have an embedded unique ID (via the jti claim). Defaults to true.")]
        public bool IncludeJwtId { get; set; }

        [Display(Name = "Client Claims Prefix",
            Description =
                "If set, the prefix client claim types will be prefixed with. Defaults to client_. The intent is to make sure they don’t accidentally collide with user claims.")]
        [Required(ErrorMessage = "Validate_Required")]
        public string ClientClaimsPrefix { get; set; } = "client_";

        [Display(Name = "Pairwise Subject Salt",
            Description =
                "Salt value used in pair-wise subjectId generation for users of this client. Currently not implemented.")]
        public string? PairWiseSubjectSalt { get; set; }

        [Display(Name = "Initiate Login URI",
            Description =
                "An optional URI that can be used to initiate login from the IdentityServer host or a third party. This is most commonly used to create a client application portal within the IdentityServer host. Defaults to null.")]
        public string? InitiateLoginUri { get; set; }

        [Display(Name = "User SSO Lifetime",
            Description =
                "The maximum duration (in seconds) since the last time the user authenticated. Defaults to null. You can adjust the lifetime of a session token to control when and how often a user is required to reenter credentials instead of being silently authenticated, when using a web application.")]
        public int? UserSsoLifetime { get; set; }

        [Display(Name = "User Code Type",
            Description = "Specifies the type of user code to use for the client. Otherwise falls back to default.")]
        public string? UserCodeType { get; set; }

        [Display(Name = "Device Code Lifetime",
            Description = "Lifetime to device code in seconds (defaults to 300 seconds / 5 minutes)")]
        public int DeviceCodeLifetime { get; set; } = 300;

        [Display(Name = "CIBA Lifetime",
            Description = "Specifies the backchannel authentication request lifetime in seconds. Defaults to null.")]
        public int? CibaLifetime { get; set; }

        [Display(Name = "Polling Interval", Description = "Backchannel polling interval in seconds. Defaults to null.")]
        public int? PollingInterval { get; set; }

        [Display(Name = "Coordinate Lifetime With User Session",
            Description =
                "When enabled, the client’s token lifetimes (e.g. refresh tokens) will be tied to the user’s session lifetime. This means when the user logs out, any revokable tokens will be removed. If using server-side sessions, expired sessions will also remove any revokable tokens, and backchannel logout will be triggered. This client’s setting overrides the global CoordinateTokensWithUserSession configuration setting.")]
        public bool? CoordinateLifetimeWithUserSession { get; set; }
    }

    public class AddPropertyModel
    {
        [Display(Name = "Key")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Key { get; set; } = null!;

        [Display(Name = "Value")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Value { get; set; } = null!;
    }
}