using System.Data;
using System.Globalization;
using AlphaId.DirectoryLogon.EntityFramework;
using AlphaId.EntityFramework;
using AlphaId.PlatformServices.Aliyun;
using AlphaId.RealName.EntityFramework;
using AlphaIdPlatform;
using AlphaIdPlatform.Debugging;
using AlphaIdPlatform.Platform;
using AlphaIdPlatform.RazorPages;
using AuthCenterWebApp;
using AuthCenterWebApp.Services;
using AuthCenterWebApp.Services.Authorization;
using BotDetect.Web;
using Duende.IdentityServer.EntityFramework.Stores;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdSubjects;
using IdSubjects.ChineseName;
using IdSubjects.DependencyInjection;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Westwind.AspNetCore.Markdown;

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
        .WriteTo.EventLog(".NET Runtime", restrictedToMinimumLevel: LogEventLevel.Information)
        .WriteTo.Logger(lc =>
        {
            lc.ReadFrom.Configuration(ctx.Configuration);
            lc.Filter.ByIncludingOnly(log =>
                {
                    if (log.Properties.TryGetValue("SourceContext", out var pv))
                    {
                        var source = JsonConvert.DeserializeObject<string>(pv.ToString());
                        if (source == "Duende.IdentityServer.Events.DefaultEventService" ||
                            source == "IdSubjects.SecurityAuditing.DefaultEventService")
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .WriteTo.MSSqlServer(
                    builder.Configuration.GetConnectionString(nameof(IdSubjectsDbContext)),
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

builder.Services.Configure<IdSubjectsOptions>(builder.Configuration.GetSection("IdSubjectsOptions"));
var idSubjectsIdentityBuilder = builder.Services.AddIdSubjectsIdentity();
    idSubjectsIdentityBuilder.IdSubjects
    .AddDefaultStores()
    .AddDbContext(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(IdSubjectsDbContext)),
            sqlOptions => { sqlOptions.UseNetTopologySuite(); });
    });

idSubjectsIdentityBuilder.IdSubjects.IdentityBuilder
    .AddSignInManager<PersonSignInManager>()
    .AddClaimsPrincipalFactory<PersonClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();//

if (bool.Parse(builder.Configuration[FeatureSwitch.RealNameFeature] ?? "false"))
{
    idSubjectsIdentityBuilder.IdSubjects.AddRealName()
        .AddDefaultStores()
        .AddDbContext(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(RealNameDbContext))));
}

if (bool.Parse(builder.Configuration[FeatureSwitch.DirectoryAccountManagementFeature] ?? "false"))
{
    idSubjectsIdentityBuilder.IdSubjects.AddDirectoryLogin()
        .AddDefaultStores()
        .AddDbContext(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(DirectoryLogonDbContext))));
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
            b.UseSqlServer(builder.Configuration.GetConnectionString("OidcConfigurationDataConnection"));
        };
    })
    .AddOperationalStore(options =>
    {
        options.EnableTokenCleanup = true;
        options.ConfigureDbContext = b =>
        {
            b.UseSqlServer(builder.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"));
        };
    })
    .AddAspNetIdentity<NaturalPerson>()
    .AddResourceOwnerValidator<PersonResourceOwnerPasswordValidator>()
    .AddServerSideSessions<ServerSideSessionStore>()
    .Services.AddTransient<IEventSink, AuditLogEventSink>();

//todo 替换 Duende.IdentityServer.Hosting.DynamicProviders.OidcConfigureOptions
builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, AdvancedOidcConfigureOptions>();

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

// 当Debug模式时，覆盖先前配置以解除外部依赖
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, NopEmailSender>();
    builder.Services.AddScoped<IShortMessageService, NopShortMessageService>();
    builder.Services.AddScoped<IVerificationCodeService, NopVerificationCodeService>();
    builder.Services.AddScoped<IChineseIdCardOcrService, DebugChineseIdCardOcrService>();
}

var app = builder.Build();

//IdentityModelEventSource.ShowPII = true;

app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseExceptionHandler("/Home/Error");
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
    ///     for Testing.
    /// </summary>
    public class Program
    {
    }
}