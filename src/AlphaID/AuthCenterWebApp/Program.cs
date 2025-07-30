using System.Data;
using System.Globalization;
using System.Threading.RateLimiting;
using AlphaId.EntityFramework.IdSubjects;
using AlphaId.PlatformServices.Aliyun;
using AlphaIdPlatform;
using AlphaIdPlatform.Debugging;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using AspNetWebLib.RazorPages;
using AuthCenterWebApp;
using AuthCenterWebApp.Middlewares;
using AuthCenterWebApp.Services;
using AuthCenterWebApp.Services.Authorization;
using AuthCenterWebApp.Services.WechatMp;
using BotDetect.Web;
using ChineseName;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.EntityFramework.Stores;
using IdSubjects;
using IdSubjects.SecurityAuditing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Westwind.AspNetCore.Markdown;
using IEventSink = Duende.IdentityServer.Services.IEventSink;
using Microsoft.OpenApi.Models;

using System.Reflection;



#if WINDOWS
using Serilog.Events;
#endif

// ReSharper disable All


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, configuration) =>
{
    configuration
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level} {EventId}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .WriteTo.Logger(lc =>
        {
            lc.ReadFrom.Configuration(ctx.Configuration);
            lc.Filter.ByIncludingOnly(log =>
                {
                    if (log.Properties.TryGetValue("SourceContext", out var pv))
                    {
                        string? source = JsonConvert.DeserializeObject<string>(pv.ToString());
                        if (source == "Duende.IdentityServer.Events.DefaultEventService" ||
                            source == "IdSubjects.SecurityAuditing.DefaultEventService")
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .WriteTo.MSSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sinkOptions: new MSSqlServerSinkOptions() { TableName = "AuditLog" },
                    columnOptions: new ColumnOptions()
                    {
                        AdditionalColumns =
                        [
                            new SqlColumn("EventId", SqlDbType.Int) { PropertyName = "EventId.Id" },
                            new SqlColumn("Source", SqlDbType.NVarChar) { PropertyName = "SourceContext" }
                        ]
                    }
                );
        });
#if WINDOWS
    configuration.WriteTo.EventLog(".NET Runtime", restrictedToMinimumLevel: LogEventLevel.Information);
#endif

});

//程序资源
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

//区域和本地化
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("zh-CN")
    };
    options.DefaultRequestCulture = new RequestCulture(culture: builder.Configuration["DefaultCulture"]!,
        uiCulture: builder.Configuration["DefaultCulture"]!);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlInfo>(builder.Configuration.GetSection("SystemUrl"));

//授权策略
builder.Services.AddAuthorizationBuilder()
          .AddPolicy("RequireOrganizationOwner", policy => { policy.Requirements.Add(new OrganizationOwnerRequirement()); });

builder.Services.AddScoped<IAuthorizationHandler, OrganizationOwnerRequirementHandler>();

builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AuthorizeFolder("/");
        options.Conventions.AuthorizeAreaFolder("Profile", "/");
        options.Conventions.AuthorizeAreaFolder("Settings", "/");
        options.Conventions.AuthorizeAreaFolder("Organization", "/Settings", "RequireOrganizationOwner");

        options.Conventions.Add(new SubjectAnchorRouteModelConvention("/", "People"));
        options.Conventions.Add(new SubjectAnchorRouteModelConvention("/", "Organization"));
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
    });

//Add AlphaIdPlatform.
var platform = builder.Services.AddAlphaIdPlatform();
platform.AddEntityFramework(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sql =>
    {
        sql.UseNetTopologySuite();
    });
});

builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection("IdentityOptions"));
builder.Services.Configure<PasswordLifetimeOptions>(builder.Configuration.GetSection("PasswordLifetimeOptions"));
builder.Services.Configure<AuditEventsOptions>(builder.Configuration.GetSection("AuditEventsOptions"));

//配置ProfileUrl
//builder.Services.Configure<OidcProfileUrlOptions>(options => options.ProfileUrlBase = new Uri(builder.Configuration["SystemUrl:AuthCenterUrl"]!));
var identityBuilder = builder.Services.AddIdSubjectsIdentity<NaturalPerson, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddUserStore<NaturalPersonStore>()
    .AddClaimsPrincipalFactory<NaturalPersonClaimsPrincipalFactory>()
    .AddEntityFrameworkStores<IdSubjectsDbContext>();

var authBuilder = builder.Services.AddAuthentication();
//添加PreSignUp方案。
authBuilder.AddCookie(AuthenticationDefaults.PreSignUpScheme, options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});

#region 配置可选外部登录
var externalLoginsSection = builder.Configuration.GetSection("ExternalLogins");
var weixinLoginSection = externalLoginsSection.GetSection("Weixin");
if (weixinLoginSection.GetValue("Enabled", false))
{
    authBuilder.AddWeixin("wechat", "微信", options =>
    {
        //替换默认的SignInScheme，以便在回调时正确验证。
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;
        options.CallbackPath = "/signin-weixin";
        options.ClientId = weixinLoginSection.GetValue("ClientId", string.Empty)!;
        options.ClientSecret = weixinLoginSection.GetValue("ClientSecret", string.Empty)!;
    });
}

var workWeixinLoginSection = externalLoginsSection.GetSection("WorkWeixin");
if (workWeixinLoginSection.GetValue("Enabled", false))
{
    authBuilder.AddWorkWeixin("signin-workweixin", "企业微信", options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;
        options.CallbackPath = "/signin-workweixin";
        options.ClientId = workWeixinLoginSection.GetValue("ClientId", string.Empty)!;
        options.ClientSecret = workWeixinLoginSection.GetValue("ClientSecret", string.Empty)!;
    });
}

var qqLoginSection = externalLoginsSection.GetSection("QQ");
if (qqLoginSection.GetValue("Enabled", false))
{
    authBuilder.AddQQ("/signin-qq", "QQ", options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;
        options.CallbackPath = "signin-qq";
        options.ClientId = qqLoginSection.GetValue("ClientId", string.Empty)!;
        options.ClientSecret = qqLoginSection.GetValue("ClientSecret", string.Empty)!;
    });
}
#endregion

#region 配置微信公众平台移动端网页授权
builder.Services.Configure<WeixinMpSettings>(builder.Configuration.GetSection("WeixinMpSettings"));
var weixinMpSettings = new WeixinMpSettings();
builder.Configuration.GetSection("WeixinMpSettings").Bind(weixinMpSettings);
if (weixinMpSettings.Enabled)
{
    authBuilder.AddWechatMp(weixinMpSettings.MpName, options =>
    {
        //替换默认的SignInScheme，以便在回调时正确验证。
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;
        //options.CallbackPath = "/signin-weixin";
        options.ClientId = weixinMpSettings.AppId;
        options.ClientSecret = weixinMpSettings.AppSecret;
    });
}
#endregion


//添加邮件发送器。
builder.Services.AddScoped<IEmailSender, SmtpMailSender>()
    .Configure<SmtpMailSenderOptions>(builder.Configuration.GetSection("SmtpMailSenderOptions"));

//添加IdentityServer
builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;

        //unit test with WebApplicationFactory should randomly occur InvalidOperationException, becaouse remove expired session task not started on unit testing.
        //Remove expired sessions when running on Production environment. pass unit test.
        options.ServerSideSessions.RemoveExpiredSessions = builder.Environment.IsProduction();

        // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
        options.EmitStaticAudienceClaim = true;

        //配置IdP标识
        options.IssuerUri = builder.Configuration["IdPConfig:IssuerUri"];

        options.ServerSideSessions.UserDisplayNameClaimType = JwtClaimTypes.Name;

        //当需要身份验证时显示的登录界面。
        options.UserInteraction.LoginUrl = "/Account/Login";
    })
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
        {
            b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        };
    })
    .AddOperationalStore(options =>
    {
        //当调试模式时，不要启用令牌清理任务。
        //否则单元测试期间会随机导致InvalidOperationException, Not started. Call Start first.
        options.EnableTokenCleanup = !builder.Environment.IsDevelopment();
        
        options.ConfigureDbContext = b =>
        {
            b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        };
    })
    .AddAspNetIdentity<NaturalPerson>()
    .AddResourceOwnerValidator<PersonResourceOwnerPasswordValidator>()
    .AddServerSideSessions<ServerSideSessionStore>()
    .Services.AddTransient<IEventSink, AuditLogEventSink>();

builder.Services.AddScoped<ChinesePersonNamePinyinConverter>();
builder.Services.AddScoped<ChinesePersonNameFactory>();

//配置登录选项
builder.Services.Configure<LoginOptions>(builder.Configuration.GetSection("LoginOptions"));

// Add Session builder.Services. 
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(20); });

//身份证OCR
builder.Services.AddScoped<IChineseIdCardOcrService, AliyunChineseIdCardOcrService>();

//xxx 由于BotDetect Captcha需要支持同步流，应改进此配置。
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

builder.Services.AddMarkdown(config =>
{
    config.AddMarkdownProcessingFolder("/docs/");
});

builder.Services.AddMvc();

//反向代理配置
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    //默认只接受来自本地主机的反向代理。
    //如果系统的网络和反向代理的部署不明确，可按下述清空KnownNetworks和KnownProxies，以接受来自任何反向代理传递的请求。
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    
});

//请求速率限制
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; //当拒绝时返回429TooManyRequests状态码
    options.AddPolicy("token-endpoint-limit", httpContext =>
    {
        var path = httpContext.Request.Path.Value!;
        if (string.Equals(path, "/connect/token", StringComparison.OrdinalIgnoreCase))
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, // 每窗口允许的请求数
                Window = TimeSpan.FromMinutes(1), // 窗口大小
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 // 队列限制
            });
        }
        return RateLimitPartition.GetNoLimiter("none");
    });
});

builder.Services.AddSwaggerGen(options =>
{
    var info = builder.Configuration.GetSection("OpenApiInfo").Get<OpenApiInfo>();
    options.SwaggerDoc("v1", info);
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
    options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["SwaggerOauthOptions:AuthorizationEndpoint"]!),
                TokenUrl = new Uri(builder.Configuration["SwaggerOauthOptions:TokenEndpoint"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "获取用户Id标识" },
                    { "profile", "获取用户基本信息" },
                    { "realname", "获取自然人的实名信息" }
                }
            }
        }
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


// 当Debug模式时，覆盖先前配置以解除外部依赖
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, NopEmailSender>();
    builder.Services.AddScoped<IShortMessageService, NopShortMessageService>();
    builder.Services.AddScoped<IVerificationCodeService, NopVerificationCodeService>();
    builder.Services.AddScoped<IChineseIdCardOcrService, DebugChineseIdCardOcrService>();
}

//配置后阶段
builder.Services.AddSingleton<IPostConfigureOptions<IdentityServerOptions>, PostConfigIdentityServerOptions>();
builder.Services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigIdentityServerOptions>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization();
app.UseMarkdown();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseMiddleware<TokenRateLimitMiddleware>(); //为token终结点启用速率限制。
app.UseIdentityServer(); //内部会调用UseAuthentication。
app.UseAuthorization();
app.UseSwagger(options => { options.RouteTemplate = "api-docs/{documentName}/docs.json"; });
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api-docs/v1/docs.json",
        $"{app.Configuration["OpenApiInfo:Title"]} {app.Configuration["OpenApiInfo:Version"]}");
    options.DocumentTitle = app.Configuration["OpenApiInfo:Title"];
    options.RoutePrefix = "api-docs";
    options.InjectStylesheet("/swagger-ui/custom.css");
    options.OAuthScopes("openid", "profile");
    options.OAuthUsePkce();
});
app.UseSession();
app.UseCaptcha(app.Configuration);
app.MapRazorPages();
app.MapControllers();

await app.RunAsync();

namespace AuthCenterWebApp
{
    /// <summary>
    /// for Testing.
    /// </summary>
    public class Program
    {
    }
}