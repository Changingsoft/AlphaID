using AdminWebApp;
using AdminWebApp.Areas.OpenIDConnect.Pages.Clients;
using AdminWebApp.Domain.Security;
using AdminWebApp.Services;
using AlphaId.EntityFramework.Admin;
using AlphaId.EntityFramework.DirectoryAccountManagement;
using AlphaId.EntityFramework.IdSubjects;
using AlphaId.EntityFramework.SecurityAuditing;
using AlphaId.PlatformServices.Aliyun;
using AlphaIdPlatform;
using AlphaIdPlatform.Admin;
using AlphaIdPlatform.Debugging;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using AspNetWebLib.RazorPages;
using ChineseName;
using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Globalization;
using System.Security.Claims;
using Westwind.AspNetCore.Markdown;

// ReSharper disable StringLiteralTypo

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region 日志
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .WriteTo.Logger(lc =>
        {
            lc.ReadFrom.Configuration(context.Configuration);
            lc.Filter.ByIncludingOnly(log =>
                {
                    if (log.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? pv))
                    {
                        var source = JsonConvert.DeserializeObject<string>(pv.ToString());
                        if (source == "Duende.IdentityServer.Events.DefaultEventService" ||
                            source == "IdSubjects.SecurityAuditing.DefaultEventService") return true;
                    }

                    return false;
                })
                .WriteTo.MSSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MSSqlServerSinkOptions { TableName = "AuditLog" },
                    columnOptions: new ColumnOptions
                    {
                        AdditionalColumns =
                        [
                            new SqlColumn("EventId", SqlDbType.Int) { PropertyName = "EventId.Id" },
                            new SqlColumn("Source", SqlDbType.NVarChar) { PropertyName = "SourceContext" }
                        ]
                    }
                );
        });
    if (OperatingSystem.IsWindows())
    {
        configuration.WriteTo.EventLog(".NET Runtime", restrictedToMinimumLevel: LogEventLevel.Information);
    }

});
#endregion

//产品和系统URL信息。
builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlInfo>(builder.Configuration.GetSection("SystemUrl"));

//程序资源
builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

//区域和本地化
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("zh-CN")
    };
    options.DefaultRequestCulture = new RequestCulture(builder.Configuration["DefaultCulture"]!,
        builder.Configuration["DefaultCulture"]!);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


#region 配置RazorPages.
builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AuthorizeFolder("/", "RequireAdminRole");
        options.Conventions.AuthorizeFolder("/Account");
        options.Conventions.Add(new SubjectAnchorRouteModelConvention("/Detail", "UserManagement"));
        options.Conventions.Add(new SubjectAnchorRouteModelConvention("/Detail", "OrganizationManagement"));
        options.Conventions.Add(new SubjectAnchorRouteModelConvention("/Clients/Detail", "OpenIDConnect"));
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource));
    });
#endregion

//启用API Controller
builder.Services.AddControllers();

//配置授权策略。
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => { policy.RequireRole(RoleConstants.AdministratorsRole.Name); });

#region 配置身份验证。
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.Authority = builder.Configuration["OidcClient:Authority"];
        options.ClientId = builder.Configuration["OidcClient:ClientId"];
        options.ClientSecret = builder.Configuration["OidcClient:ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        //options.Scope.Add("membership"); //已默认包括openid和profile。
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true; //从UserInfoEndPoint取得更多用户信息。

        //hack 将name声明添加到完全类型“http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name”上。
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, JwtClaimTypes.Name);
        //options.ClaimActions.MapJsonKey("profile", JwtClaimTypes.Profile);
        options.ClaimActions.MapJsonKey("picture", JwtClaimTypes.Picture);
        options.ClaimActions.MapJsonKey("locale", JwtClaimTypes.Locale);
        options.ClaimActions.MapJsonKey("zoneinfo", JwtClaimTypes.ZoneInfo);

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = OidcEvents.IssueRoleClaims
        };
    });
#endregion

#region 配置AlphaID平台服务。
var platform = builder.Services.AddAlphaIdPlatform();
platform.AddEntityFramework(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sql =>
    {
        sql.UseNetTopologySuite();
    });
});
builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection("IdentityOptions"));

//DbContext for Duende IdentityServer.
builder.Services.AddDbContext<ConfigurationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}).AddScoped<ConfigurationStoreOptions>();
builder.Services.AddDbContext<PersistedGrantDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}).AddScoped<OperationalStoreOptions>();

builder.Services.AddDbContext<OperationalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(OperationalDbContext)));
});
#endregion

//身份证OCR识别
builder.Services.AddScoped<IChineseIdCardOcrService, AliyunChineseIdCardOcrService>();


builder.Services.AddScoped<ChinesePersonNamePinyinConverter>();
builder.Services.AddScoped<ChinesePersonNameFactory>();

builder.Services.AddScoped<DirectoryAccountManager<NaturalPerson>>()
    .AddScoped<IDirectoryAccountStore, DirectoryAccountStore>()
    .AddScoped<IDirectoryServiceStore, DirectoryServiceStore>();


//添加邮件发送器。
builder.Services.AddScoped<IEmailSender, SmtpMailSender>()
    .Configure<SmtpMailSenderOptions>(builder.Configuration.GetSection("SmtpMailSenderOptions"));

//目录服务
builder.Services.AddScoped<DirectoryServiceManager>()
    .AddScoped<IDirectoryServiceStore, DirectoryServiceStore>();
builder.Services.AddScoped<DirectoryAccountManager<NaturalPerson>>()
    .AddScoped<IDirectoryAccountStore, DirectoryAccountStore>();

//安全角色管理
builder.Services.AddScoped<UserInRoleManager>()
    .AddScoped<IUserInRoleStore, UserInRoleStore>();

builder.Services.AddScoped<ISecretGenerator, DefaultSecretGenerator>();


builder.Services.AddMarkdown();

#region 反向代理配置
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    //默认只接受来自本地主机的反向代理。
    //如果系统的网络和反向代理的部署不明确，可按下述清空KnownNetworks和KnownProxies，以接受来自任何反向代理传递的请求。
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
#endregion

//当Debug模式时，覆盖注册先前配置以解除外部依赖
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, NopEmailSender>();
    builder.Services.AddScoped<IShortMessageService, NopShortMessageService>();
    builder.Services.AddScoped<IVerificationCodeService, NopVerificationCodeService>();
    builder.Services.AddScoped<IChineseIdCardOcrService, DebugChineseIdCardOcrService>();
}

//安全审计日志
builder.Services.AddAuditLog()
    .AddDefaultStore()
    .AddDbContext(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

WebApplication app = builder.Build();

app.UseForwardedHeaders();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseMarkdown();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseSession();
app.MapRazorPages();
app.MapControllers();
//app.MapBlazorHub();

app.Run();

namespace AdminWebApp
{
    public class Program;
}