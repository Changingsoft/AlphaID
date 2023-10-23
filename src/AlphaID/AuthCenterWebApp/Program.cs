using AlphaIDEntityFramework.EntityFramework;
using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using AlphaIDPlatformServices.Aliyun;
using AlphaIDPlatformServices.Primitives;
using AuthCenterWebApp;
using AuthCenterWebApp.Services;
using Azure.Identity;
using BotDetect.Web;
using Duende.IdentityServer.EntityFramework.Stores;
using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;
using System.Globalization;

Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger(); //hack see https://github.com/serilog/serilog-aspnetcore/issues/289#issuecomment-1060303792

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                .WriteTo.EventLog(".NET Runtime", manageEventSource: true)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(ctx.Configuration);
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
        options.Conventions.AddAreaFolderRouteModelConvention("People", "/", model =>
        {
            var viewPath = model.ViewEnginePath;
            var endPart = viewPath.LastIndexOf("/index", StringComparison.OrdinalIgnoreCase);
            if (endPart >= 0)
                viewPath = viewPath[..endPart];

            var template = $"/{model.AreaName}/{{userAnchor}}{viewPath}";

            var metadata = new PageRouteMetadata($"/{model.AreaName}{viewPath}/{{userAnchor}}", template);
            var selector = new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel()
                {
                    //Order = 2,
                    Template = template,
                }
            };
            selector.EndpointMetadata.Add(metadata);
            model.Selectors.Clear();
            model.Selectors.Add(selector);
        });
        options.Conventions.AddAreaFolderRouteModelConvention("Organization", "/", model =>
        {
            var viewPath = model.ViewEnginePath;
            var endPart = viewPath.LastIndexOf("/index", StringComparison.OrdinalIgnoreCase);
            if (endPart >= 0)
                viewPath = viewPath[..endPart];

            var template = $"/{model.AreaName}/{{anchor}}{viewPath}";

            var metadata = new PageRouteMetadata($"/{model.AreaName}{viewPath}/{{anchor}}", template);
            var selector = new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel()
                {
                    //Order = 2,
                    Template = template,
                }
            };
            selector.EndpointMetadata.Add(metadata);
            model.Selectors.Clear();
            model.Selectors.Add(selector);
        });
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
        options.UseLazyLoadingProxies();
    });

    //启用AspNetCore.IdentityCore
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme, o =>
    {
        o.LoginPath = new PathString("/Account/Login");
        o.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
        };
    })
    .AddCookie(IdentityConstants.ExternalScheme, o =>
    {
        o.Cookie.Name = IdentityConstants.ExternalScheme;
        o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
    {
        o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
        o.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
        };
    })
    .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
    {
        o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
        o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    })
    //添加必须修改密码cookie。
    .AddCookie(AuthCenterIdentitySchemes.MustChangePasswordScheme, o =>
    {
        o.Cookie.Name = AuthCenterIdentitySchemes.MustChangePasswordScheme;
        o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddIdentityCore<NaturalPerson>()
        .AddUserManager<NaturalPersonManager>()
        .AddSignInManager<PersonSignInManager>()
        .AddUserStore<NaturalPersonStore>()
        .AddClaimsPrincipalFactory<PersonClaimsPrincipalFactory>()
        .AddUserValidator<NaturalPersonValidator>()
        .AddDefaultTokenProviders();
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

    //组织成员
    builder.Services.AddScoped<OrganizationMemberManager>()
        .AddScoped<IOrganizationMemberStore, OrganizationMemberStore>();

    //Organizations
    builder.Services.AddScoped<OrganizationManager>()
        .AddScoped<IOrganizationStore, OrganizationStore>();

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
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException") // https://github.com/dotnet/runtime/issues/60600
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

namespace AuthCenterWebApp
{
    /// <summary>
    /// Definations for Testing.
    /// </summary>
    public partial class Program { }
}