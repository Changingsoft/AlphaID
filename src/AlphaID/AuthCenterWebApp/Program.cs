using AlphaID.EntityFramework;
using AlphaID.PlatformServices.Aliyun;
using AlphaID.PlatformServices.Primitives;
using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using AlphaIDPlatform.RazorPages;
using AuthCenterWebApp;
using AuthCenterWebApp.Services;
using BotDetect.Web;
using Duende.IdentityServer.EntityFramework.Stores;
using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, configuration) =>
{
    configuration
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .WriteTo.EventLog(".NET Runtime", manageEventSource: true);
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
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AuthorizeAreaFolder("Profile", "/");
    options.Conventions.AuthorizeAreaFolder("Settings", "/");
    options.Conventions.Add(new SubjectAnchorRouteModelConvention("/", "People", "{userAnchor}"));
    options.Conventions.Add(new SubjectAnchorRouteModelConvention("/", "Organization"));
})
.AddViewLocalization()
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
});

builder.Services.AddDbContext<IDSubjectsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection"), sqlOptions =>
    {
        sqlOptions.UseNetTopologySuite();
    });
    options.UseLazyLoadingProxies(); //hack disable lazy loading, about cause issue identity.
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddIdSubjectsIdentity(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-";
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
    .AddSignInManager<PersonSignInManager>()
    .AddClaimsPrincipalFactory<PersonClaimsPrincipalFactory>()
    .AddDefaultTokenProviders()
    .AddDefaultStores();

builder.Services.AddScoped<IQueryableUserStore<NaturalPerson>, NaturalPersonStore>();

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
//配置IdentityServer的配置存储
.AddConfigurationStore(options =>
{
    options.ConfigureDbContext = b =>
    {
        b.UseSqlServer(builder.Configuration.GetConnectionString("OidcConfigurationDataConnection"), o =>
        {
            o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
        });
    };
})
//操作记录存储
.AddOperationalStore(options =>
{
    options.ConfigureDbContext = b =>
    {
        b.UseSqlServer(builder.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"), o =>
        {
            o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
        });
    };
})
//使用AspNetIdentity.
.AddAspNetIdentity<NaturalPerson>()
.AddResourceOwnerValidator<PersonResourceOwnerPasswordValidator>()
.AddServerSideSessions<ServerSideSessionStore>();

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

//实名认证
builder.Services.AddScoped<ChineseIDCardManager>()
    .AddScoped<IChineseIDCardValidationStore, RealNameValidationStore>();

//身份证OCR
builder.Services.AddScoped<IChineseIDCardOCRService, AliyunChineseIDCardOCRService>();

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

//测试获取EmailSender
using (var scope = app.Services.CreateScope())
{
    var emailSenders = scope.ServiceProvider.GetService<IEnumerable<IEmailSender>>();
    Debug.Assert(emailSenders != null);
    Debug.Assert(emailSenders.Count() == 2); //will be contained 2 senders.
}

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
    /// Definations for Testing.
    /// </summary>
    public partial class Program { }
}