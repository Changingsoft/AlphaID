using AlphaIDEntityFramework.EntityFramework;
using AlphaIDEntityFramework.EntityFramework.Identity;
using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using AuthCenterWebApp.Services;
using AuthCenterWebApp.Services.XinAn;
using BotDetect.Web;
using Duende.IdentityServer;
using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Json;
using Serilog;

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
            services.Configure<ProductInfo>(context.Configuration.GetSection("ProductInfo"));
            services.Configure<SystemUrlOptions>(context.Configuration.GetSection("SystemUrl"));
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/");
                //options.Conventions.AllowAnonymousToFolder("/Account");
            });

            services.AddDbContext<IDSubjectsDbContext>(options =>
            {
                options.UseSqlServer(context.Configuration.GetConnectionString("IDSubjectsDataConnection"));
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
            services.AddScoped<INaturalPersonStore, NaturalPersonStore>();

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
                .AddServerSideSessions();

            //启用身份验证
            services.AddAuthentication()
                .AddOpenIdConnect("federal.changingsoft.com", "员工登录", options =>
                {
                    options.MetadataAddress = "https://federal.changingsoft.com/adfs/.well-known/openid-configuration";
                    options.ClientId = "8cdd4862-37bd-462f-a1cc-698c59a0c7fe";
                    options.ClientSecret = "f7myF3915f9rPrARsDH21OeX70BxSRkp6CI2tNm7";
                    //options.ResponseType = ResponseTypes.Code;
                    //options.ResponseMode = ResponseModes.Query;
                    //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.CallbackPath = "/signin-adfs";
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Resource = "https://auth.changingsoft.com/resource"; //提示Resource以便将其他声明添加到id_token中
                                                                              //options.Scope.Add("allatclaims");
                                                                              //options.Scope.Add("email");

                    options.SaveTokens = true;
                    //在调用注销的参数中添加id_token_hint，以便注销时能够回到本应用。适用于AD FS。
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProviderForSignOut = async context =>
                        {
                            context.ProtocolMessage.SetParameter("id_token_hint", await context.HttpContext.GetTokenAsync("id_token"));
                        }
                    };
                })
                .AddXinAn("netauth.changingsoft.com", "信安世纪SSO", options =>
                {
                    //添加信安世纪的单点登录节点。
                    //节点管理人 信安世纪 夏文良 18595462619
                    options.ClientId = context.Configuration["XinanSSO:ClientId"]!;
                    options.ClientSecret = context.Configuration["XinanSSO:ClientSecret"]!;
                    options.UsePkce = false;
                    options.SaveTokens = true;
                })
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