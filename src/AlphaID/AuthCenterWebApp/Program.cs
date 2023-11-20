using AlphaId.DirectoryLogon.EntityFramework;
using AlphaId.EntityFramework;
using AlphaId.PlatformServices.Aliyun;
using AlphaId.PlatformServices.Primitives;
using AlphaId.RealName.EntityFramework;
using AlphaIdPlatform;
using AlphaIdPlatform.Platform;
using AlphaIdPlatform.RazorPages;
using AuthCenterWebApp;
using AuthCenterWebApp.Services;
using AuthCenterWebApp.Services.Authorization;
using BotDetect.Web;
using Duende.IdentityServer.EntityFramework.Stores;
using IdSubjects;
using IdSubjects.ChineseName;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Globalization;
// ReSharper disable All


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, configuration) =>
{
    configuration
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level} {EventId}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .WriteTo.EventLog(".NET Runtime", restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.Logger(lc =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration);
                lc.Filter.ByIncludingOnly(log =>
                    {
                        if (log.Properties.TryGetValue("SourceContext", out var pv))
                        {
                            var source = JsonConvert.DeserializeObject<string>(pv.ToString());
                            if (source == "Duende.IdentityServer.Events.DefaultEventService" || source == "IdSubjects.SecurityAuditing.DefaultEventService")
                            {
                                return true;
                            }
                        }
                        return false;
                    })
                    .WriteTo.MSSqlServer(
                        builder.Configuration.GetConnectionString("IDSubjectsDataConnection"),
                        sinkOptions: new MSSqlServerSinkOptions() { TableName = "AuditLog" },
                        columnOptions: new ColumnOptions()
                        {
                            AdditionalColumns = new[]
                            {
                                new SqlColumn("EventId", SqlDbType.Int){PropertyName = "EventId.Id"},
                                new SqlColumn("Source", SqlDbType.NVarChar){PropertyName = "SourceContext"},
                            },
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
                    new CultureInfo("zh-CN"),
    };
    options.DefaultRequestCulture = new RequestCulture(culture: builder.Configuration["DefaultCulture"]!, uiCulture: builder.Configuration["DefaultCulture"]!);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlInfo>(builder.Configuration.GetSection("SystemUrl"));

//授权策略
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireOrganizationOwner", policy =>
    {
        policy.Requirements.Add(new OrganizationOwnerRequirement());
    });
});
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


var idSubjectsBuilder = builder.Services.AddIdSubjectsIdentity(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789.-_@";
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.ChangePasswordColdDown = 5;
    options.Password.EnablePassExpires = true;
    options.Password.RememberPasswordHistory = 1;
})
    .AddDefaultStores()
    .AddDbContext(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection"), sqlOptions =>
        {
            sqlOptions.UseNetTopologySuite();
        });
    });

idSubjectsBuilder.IdentityBuilder
    .AddSignInManager<PersonSignInManager>()
    .AddClaimsPrincipalFactory<PersonClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();

if (true) //todo 设置一个开关以决定是否启用实名认证模块
{
    idSubjectsBuilder.AddRealName()
        .AddDefaultStores()
        .AddDbContext(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection")));
}

if (true)
{
    idSubjectsBuilder.AddDirectoryLogin()
        .AddDefaultStores()
        .AddDbContext(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection")));
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

        //hack 将外部登录的方案修改为AspNetCoreIdentity的默认值？
        options.DynamicProviders.SignInScheme = IdentityConstants.ExternalScheme;
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
        options.ConfigureDbContext = b =>
        {
            b.UseSqlServer(builder.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"));
        };
    })
    .AddAspNetIdentity<NaturalPerson>()
    .AddResourceOwnerValidator<PersonResourceOwnerPasswordValidator>()
    .AddServerSideSessions<ServerSideSessionStore>()
    .Services.AddTransient<Duende.IdentityServer.Services.IEventSink, AuditLogEventSink>();

//短信服务
builder.Services.AddScoped<IShortMessageService, SimpleShortMessageService>();
builder.Services.AddScoped<IVerificationCodeService, SimpleShortMessageService>();
builder.Services.Configure<SimpleShortMessageServiceOptions>(options =>
{
    options.ClientId = "bbb867eb-f1e2-4deb-8a21-832f963b4a74";
    options.ClientSecret = "XIKHAcDO6oVYIAQQs8cewfaJwGxVV5u5x-6Yi-lu";
});

builder.Services.AddScoped<ChinesePersonNamePinyinConverter>();
builder.Services.AddScoped<ChinesePersonNameFactory>();

// Add Session builder.Services. 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

//身份证OCR
builder.Services.AddScoped<IChineseIdCardOcrService, AliyunChineseIdCardOcrService>();

//todo 由于BotDetect Captcha需要支持同步流，应改进此配置。
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

//当Debug模式时，覆盖注册先前配置以解除外部依赖
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, NopEmailSender>();
    builder.Services.AddScoped<IShortMessageService, NopShortMessageService>();
    builder.Services.AddScoped<IVerificationCodeService, NopVerificationCodeService>();
}

var app = builder.Build();

app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRequestLocalization();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer(); //内部会调用UseAuthentication。
app.UseAuthorization();
app.UseSession();
app.UseCaptcha(app.Configuration);
app.MapRazorPages();

await app.RunAsync();

namespace AuthCenterWebApp
{
    /// <summary>
    /// Definitions for Testing.
    /// </summary>
    public class Program { }
}