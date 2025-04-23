using System.Data;
using System.Globalization;
using AlphaId.EntityFramework.IdSubjects;
using AlphaId.PlatformServices.Aliyun;
using AlphaIdPlatform;
using AlphaIdPlatform.Debugging;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using AspNetWebLib.RazorPages;
using AuthCenterWebApp;
using AuthCenterWebApp.Services;
using AuthCenterWebApp.Services.Authorization;
using BotDetect.Web;
using Duende.IdentityServer.EntityFramework.Stores;
using IdentityModel;
using IdSubjects;
using IdSubjects.ChineseName;
using IdSubjects.SecurityAuditing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Westwind.AspNetCore.Markdown;
using IEventSink = Duende.IdentityServer.Services.IEventSink;
using Microsoft.AspNetCore.HttpOverrides;

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
builder.Services.Configure<OidcProfileUrlOptions>(options => options.ProfileUrlBase = new Uri(builder.Configuration["SystemUrl:AuthCenterUrl"]!));
var identityBuilder = builder.Services.AddIdSubjectsIdentity<NaturalPerson, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddSignInManager<ApplicationUserSignInManager<NaturalPerson>>()
    .AddUserStore<NaturalPersonStore>()
    .AddClaimsPrincipalFactory<NaturalPersonClaimsPrincipalFactory>()
    .AddEntityFrameworkStores<IdSubjectsDbContext>();

//配置外部登录。
var authBuilder = builder.Services.AddAuthentication();
var externalLoginsSection = builder.Configuration.GetSection("ExternalLogins");
var weixinLoginSection = externalLoginsSection.GetSection("Weixin");
if (weixinLoginSection.GetValue("Enabled", false))
{
    authBuilder.AddWeixin("signin-weixin", "微信", options =>
    {
        options.CallbackPath = "signin-weixin";
        options.ClientId = weixinLoginSection.GetValue("ClientId", string.Empty)!;
        options.ClientSecret = weixinLoginSection.GetValue("ClientSecret", string.Empty)!;
    });
}

var workWeixinLoginSection = externalLoginsSection.GetSection("WorkWeixin");
if (workWeixinLoginSection.GetValue("Enabled", false))
{
    authBuilder.AddWorkWeixin("signin-workweixin", "企业微信", options =>
    {
        options.CallbackPath = "signin-workweixin";
        options.ClientId = workWeixinLoginSection.GetValue("ClientId", string.Empty)!;
        options.ClientSecret = workWeixinLoginSection.GetValue("ClientSecret", string.Empty)!;
    });
}

var qqLoginSection = externalLoginsSection.GetSection("QQ");
if (qqLoginSection.GetValue("Enabled", false))
{
    authBuilder.AddQQ("signin-qq", "QQ", options =>
    {
        options.CallbackPath = "signin-qq";
        options.ClientId = qqLoginSection.GetValue("ClientId", string.Empty)!;
        options.ClientSecret = qqLoginSection.GetValue("ClientSecret", string.Empty)!;
    });
}

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

        // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
        options.EmitStaticAudienceClaim = true;

        //配置IdP标识
        options.IssuerUri = builder.Configuration["IdPConfig:IssuerUri"];

        options.ServerSideSessions.UserDisplayNameClaimType = JwtClaimTypes.Name;
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
        options.EnableTokenCleanup = true;
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

builder.Services.AddMarkdown(config => { config.AddMarkdownProcessingFolder("/_docs/"); });
builder.Services.AddMvc();

//反向代理配置
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    //默认只接受来自本地主机的反向代理。
    //如果系统的网络和反向代理的部署不明确，可按下述清空KnownNetworks和KnownProxies，以接受来自任何反向代理传递的请求。
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 当Debug模式时，覆盖先前配置以解除外部依赖
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, NopEmailSender>();
    builder.Services.AddScoped<IShortMessageService, NopShortMessageService>();
    builder.Services.AddScoped<IVerificationCodeService, NopVerificationCodeService>();
    builder.Services.AddScoped<IChineseIdCardOcrService, DebugChineseIdCardOcrService>();
}

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
app.UseIdentityServer(); //内部会调用UseAuthentication。
app.UseAuthorization();
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