using AdminWebApp;
using AdminWebApp.Domain.Security;
using AdminWebApp.Infrastructure.DataStores;
using AdminWebApp.Services;
using AlphaIDEntityFramework.EntityFramework;
using AlphaIDEntityFramework.EntityFramework.Identity;
using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using DirectoryLogon;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using IdentityModel;
using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//ConfigServices
builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlOptions>(builder.Configuration.GetSection("SystemUrl"));

//程序资源
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

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


//配置RazorPages.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/", "RequireAdminRole");
    options.Conventions.AuthorizeFolder("/Account");
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
    })
    .AddSessionStateTempDataProvider();

//启用API Controller
builder.Services.AddControllers();

//配置授权策略。
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole(RoleConstants.AdministratorsRole.Name);
    });
});

//启用服务器Session
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

//配置身份验证。
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.MetadataAddress = builder.Configuration["OidcClient:MetadataAddress"];
        options.ClientId = builder.Configuration["OidcClient:ClientId"];
        options.ClientSecret = builder.Configuration["OidcClient:ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true; //从UserInfoEndPoint取得更多用户信息。

        //hack 将name声明添加到完全类型“http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name”上。
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, JwtClaimTypes.Name);
        //options.ClaimActions.MapJsonKey("profile", JwtClaimTypes.Profile);
        options.ClaimActions.MapJsonKey("picture", JwtClaimTypes.Picture);
        options.ClaimActions.MapJsonKey("locale", JwtClaimTypes.Locale);
        options.ClaimActions.MapJsonKey("zoneinfo", JwtClaimTypes.ZoneInfo);

        options.Events = new OpenIdConnectEvents()
        {
            OnRedirectToIdentityProvider = context =>
            {
                if (!builder.Environment.IsDevelopment())
                {
                    var from = context.Request.Query["from"];
                    if (from.Contains("netauth.changingsoft.com"))
                    {
                        context.ProtocolMessage.SetParameter("acr_values", "idp:netauth.changingsoft.com");
                    }
                    else
                    {
                        //指示认证中心直接使用AD FS来处理用户登录。
                        //context.ProtocolMessage.SetParameter("acr_values", "idp:federal.changingsoft.com");
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

//注册IDSubjects DbContext.
builder.Services.AddDbContext<IDSubjectsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection"), sqlOptions =>
    {
        sqlOptions.UseNetTopologySuite();
    });
    options.UseLazyLoadingProxies();
});
builder.Services.AddDbContext<DirectoryLogonDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DirectoryLogonDataConnection"));
    options.UseLazyLoadingProxies();
});
builder.Services.AddDbContext<ConfigurationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OidcConfigurationDataConnection"));
}).AddScoped<ConfigurationStoreOptions>();
builder.Services.AddDbContext<PersistedGrantDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"));
    options.UseLazyLoadingProxies();
});

builder.Services.AddDbContext<OperationalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OperationalDataConnection"));
    options.UseLazyLoadingProxies();
});

//自然人管理器
builder.Services.AddIdentityCore<NaturalPerson>(options =>
{

})
    .AddUserManager<NaturalPersonManager>()
    .AddUserStore<NaturalPersonStore>()
    .AddDefaultTokenProviders();


//实名身份验证器。
builder.Services.AddScoped<ChineseIDCardManager>()
    .AddScoped<IChineseIDCardValidationStore, RealNameValidationStore>();

//身份证OCR识别
builder.Services.AddScoped<IChineseIDCardOCRService, AliyunChineseIDCardOCRService>();

//组织管理器
builder.Services.AddScoped<OrganizationManager>()
    .AddScoped<IQueryableOrganizationStore, OrganizationStore>()
    .AddScoped<IOrganizationStore, OrganizationStore>()
    .AddScoped<IQueryableOrganizationUsedNameStore, OrganizationUsedNameStore>();
builder.Services.AddScoped<OrganizationSearcher>();

//组织成员
builder.Services.AddScoped<OrganizationMemberManager>()
    .AddScoped<IOrganizationMemberStore, OrganizationMemberStore>();

builder.Services.AddScoped<ChinesePersonNamePinyinConverter>();
builder.Services.AddScoped<ChinesePersonNameFactory>();

builder.Services.AddScoped<LogonAccountManager>()
    .AddScoped<ILogonAccountStore, LogonAccountStore>()
    .AddScoped<IQueryableLogonAccountStore, QueryableLogonAccountStore>()
    .AddScoped<IDirectoryServiceStore, DirectoryServiceStore>();

//短信、短信验证码服务
builder.Services.AddScoped<IShortMessageService, SimpleShortMessageService>();
builder.Services.AddScoped<IVerificationCodeService, SimpleShortMessageService>();
builder.Services.Configure<SimpleShortMessageServiceOptions>(options =>
{
    options.ClientId = "bbb867eb-f1e2-4deb-8a21-832f963b4a74";
    options.ClientSecret = "XIKHAcDO6oVYIAQQs8cewfaJwGxVV5u5x-6Yi-lu";
});

//添加邮件发送器。
builder.Services.AddScoped<IEmailSender, SmtpMailSender>()
    .Configure<SmtpMailSenderOptions>(builder.Configuration.GetSection("SmtpMailSenderOptions"));

builder.Services.AddScoped<IdApiService>();


//令牌转换服务
builder.Services.AddScoped<IClaimsTransformation, ClaimTransformation>();

//目录服务
builder.Services.AddScoped<DirectoryServiceManager>()
    .AddScoped<IDirectoryServiceStore, DirectoryServiceStore>();
builder.Services.AddScoped<LogonAccountManager>()
    .AddScoped<ILogonAccountStore, LogonAccountStore>();

//安全角色管理
builder.Services.AddScoped<UserInRoleManager>()
    .AddScoped<IUserInRoleStore, UserInRoleStore>();

//当Debug模式时，覆盖注册先前配置以解除外部依赖
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, NopEmailSender>();
    builder.Services.AddScoped<IShortMessageService, NopShortMessageService>();
    builder.Services.AddScoped<IVerificationCodeService, NopVerificationCodeService>();
}


var app = builder.Build();

//Pipelines.
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
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.MapRazorPages();
app.MapControllers();

var startAction = app.Configuration["StartAction"];
if(startAction != null)
{
    switch(startAction.ToLower())
    {
        case "migration":
            using (var db = app.Services.GetRequiredService<OperationalDbContext>())
            {
                db.Database.Migrate();
            }
            break;
    }
}

//Run
app.Run();

namespace AdminWebApp
{
    public partial class Program { }
}