using AlphaIDEntityFramework.EntityFramework;
using AlphaIDEntityFramework.EntityFramework.Identity;
using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
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
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using System.Globalization;

Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var hostBuilder = Host.CreateDefaultBuilder(args);
    hostBuilder.ConfigureAppConfiguration((context, config) =>
    {
        var jsonFileSources = config.Sources.Where(p => p.GetType() == typeof(JsonConfigurationSource)).ToList();
        foreach (var source in jsonFileSources)
        {
            config.Sources.Remove(source);
        }
        config.AddJsonFile("AuthCenterSettings.json", false, false);
        config.AddJsonFile($"AuthCenterSettings.{context.HostingEnvironment.EnvironmentName}.json", true, false);
        config.AddEnvironmentVariables();
    });
    hostBuilder.ConfigureWebHostDefaults(config =>
    {
        config.ConfigureServices((context, services) =>
        {
            //程序资源
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            //区域和本地化
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN"),
                };
                options.DefaultRequestCulture = new RequestCulture(culture: context.Configuration["DefaultCulture"]!, uiCulture: context.Configuration["DefaultCulture"]!);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.Configure<ProductInfo>(context.Configuration.GetSection("ProductInfo"));
            services.Configure<SystemUrlOptions>(context.Configuration.GetSection("SystemUrl"));
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/");
                options.Conventions.AuthorizeAreaFolder("Profile", "/");
                //options.Conventions.AllowAnonymousToFolder("/Account");
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
            });




            services.AddDbContext<IDSubjectsDbContext>(options =>
            {
                options.UseSqlServer(context.Configuration.GetConnectionString("IDSubjectsDataConnection"),sqlOptions =>
                {
                    sqlOptions.UseNetTopologySuite();
                });
                options.UseLazyLoadingProxies();
            });

            //启用AspNetCore.Identity
            services.AddIdentity<NaturalPerson, IdentityRole>()
                .AddUserStore<NaturalPersonStore>()
                .AddRoleStore<FakeRoleStore>() //不起作用的RoleStore.
                .AddUserManager<NaturalPersonManager>()
                .AddSignInManager<PersonSignInManager>()
                .AddClaimsPrincipalFactory<PersonClaimsPrincipalFactory>()
                .AddUserValidator<NaturalPersonValidator>()
                .AddDefaultTokenProviders();
            //services.AddScoped<INaturalPersonStore, NaturalPersonStore>();

            //添加邮件发送器。
            services.AddScoped<IEmailSender, SmtpMailSender>()
                .Configure<SmtpMailSenderOptions>(context.Configuration.GetSection("SmtpMailSenderOptions"));

            //添加IdentityServer
            services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                    options.EmitStaticAudienceClaim = true;

                    //配置IdP标识
                    options.IssuerUri = context.Configuration["IdPConfig:IssuerUri"];

                    //hack 将外部登录的方案修改为AspNetCoreIdentity的默认值？
                    options.DynamicProviders.SignInScheme = IdentityConstants.ExternalScheme;
                })
                //配置IdentityServer的配置存储
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(context.Configuration.GetConnectionString("OidcConfigurationDataConnection"), o =>
                        {
                            o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        });
                    };
                })
                //操作记录存储
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(context.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"), o =>
                        {
                            o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        });
                    };
                })
                //使用AspNetIdentity.
                .AddAspNetIdentity<NaturalPerson>()
                .AddResourceOwnerValidator<PersonResourceOwnerPasswordValidator>()
                .AddServerSideSessions<ServerSideSessionStore>();

            //启用身份验证
            services.AddAuthentication()
                //添加必须修改密码cookie。
                .AddCookie(AuthCenterIdentitySchemes.MustChangePasswordScheme, o =>
                {
                    o.Cookie.Name = AuthCenterIdentitySchemes.MustChangePasswordScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                });

            //短信服务
            services.AddScoped<IShortMessageService, SimpleShortMessageService>();
            services.AddScoped<IVerificationCodeService, SimpleShortMessageService>();
            services.Configure<SimpleShortMessageServiceOptions>(options =>
            {
                options.ClientId = "bbb867eb-f1e2-4deb-8a21-832f963b4a74";
                options.ClientSecret = "XIKHAcDO6oVYIAQQs8cewfaJwGxVV5u5x-6Yi-lu";
            });

            services.AddScoped<ChinesePersonNamePinyinConverter>();
            services.AddScoped<ChinesePersonNameFactory>();

            // Add Session services. 
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            //实名认证
            services.AddScoped<ChineseIDCardManager>()
                .AddScoped<IChineseIDCardValidationStore, RealNameValidationStore>();

            //身份证OCR
            services.AddScoped<IChineseIDCardOCRService, AliyunChineseIDCardOCRService>();

            //组织成员
            services.AddScoped<OrganizationMemberManager>()
                .AddScoped<IOrganizationMemberStore, OrganizationMemberStore>();

            //todo 由于BotDetect Captcha需要支持同步流，应改进此配置。
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            //当Debug模式时，覆盖注册先前配置以解除外部依赖
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddScoped<IEmailSender, NopEmailSender>();
                services.AddScoped<IShortMessageService, NopShortMessageService>();
                services.AddScoped<IVerificationCodeService, NopVerificationCodeService>();
            }

        });
        config.Configure((context, app) =>
        {
            app.UseSerilogRequestLogging();

            if (context.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer(); //内部会调用UseAuthentication。
            app.UseAuthorization();
            app.UseSession();
            app.UseCaptcha(context.Configuration);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        });
    });

    hostBuilder.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .WriteTo.EventLog(".NET Runtime", manageEventSource: true)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    var host = hostBuilder.Build();



    await host.RunAsync();


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