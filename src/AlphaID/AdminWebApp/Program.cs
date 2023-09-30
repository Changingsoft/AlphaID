using AdminWebApp.Services;
using AlphaIDEntityFramework.EntityFramework;
using AlphaIDEntityFramework.EntityFramework.Identity;
using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using AlphaIDPlatform.Security;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OperationalEF;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//ConfigServices
builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlOptions>(builder.Configuration.GetSection("SystemUrl"));

//����RazorPages.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/", "RequireAdminRole");
    options.Conventions.AuthorizeFolder("/Account");
})
    .AddSessionStateTempDataProvider();

//����API Controller
builder.Services.AddControllers();

//������Ȩ���ԡ�
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole(RoleConstants.AdministratorsRole.Name);
    });
});

//���÷�����Session
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

//���������֤��
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
        options.GetClaimsFromUserInfoEndpoint = true; //��UserInfoEndPointȡ�ø����û���Ϣ��

        //hack ��name������ӵ���ȫ���͡�http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name���ϡ�
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, JwtClaimTypes.Name);
        //options.ClaimActions.MapJsonKey("profile", JwtClaimTypes.Profile);
        options.ClaimActions.MapJsonKey("avator", JwtClaimTypes.Picture);

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
                        //ָʾ��֤����ֱ��ʹ��AD FS�������û���¼��
                        //context.ProtocolMessage.SetParameter("acr_values", "idp:federal.changingsoft.com");
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

//ע��IDSubjects DbContext.
builder.Services.AddDbContext<IDSubjectsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection"));
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

//��Ȼ�˹�����
builder.Services.AddIdentityCore<NaturalPerson>(options =>
{

})
    .AddUserManager<NaturalPersonManager>()
    .AddUserStore<NaturalPersonStore>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<INaturalPersonImageStore, NaturalPersonImageStore>();

//ʵ�������֤����
builder.Services.AddScoped<ChineseIDCardManager>()
    .AddScoped<IChineseIDCardValidationStore, RealNameValidationStore>();

//���֤OCRʶ��
builder.Services.AddScoped<IChineseIDCardOCRService, AliyunChineseIDCardOCRService>();

//��֯������
builder.Services.AddScoped<OrganizationManager>()
    .AddScoped<IQueryableOrganizationStore, OrganizationStore>()
    .AddScoped<IOrganizationStore, OrganizationStore>()
    .AddScoped<IQueryableOrganizationUsedNameStore, OrganizationUsedNameStore>();
builder.Services.AddScoped<OrganizationSearcher>();

//��֯��Ա
builder.Services.AddScoped<OrganizationMemberManager>()
    .AddScoped<IOrganizationMemberStore, OrganizationMemberStore>();

builder.Services.AddScoped<ChinesePersonNamePinyinConverter>();
builder.Services.AddScoped<ChinesePersonNameFactory>();

builder.Services.AddScoped<LogonAccountManager>()
    .AddScoped<ILogonAccountStore, LogonAccountStore>()
    .AddScoped<IQueryableLogonAccountStore, QueryableLogonAccountStore>()
    .AddScoped<IDirectoryServiceStore, DirectoryServiceStore>();

//���š�������֤�����
builder.Services.AddScoped<IShortMessageService, SimpleShortMessageService>();
builder.Services.AddScoped<IVerificationCodeService, SimpleShortMessageService>();
builder.Services.Configure<SimpleShortMessageServiceOptions>(options =>
{
    options.ClientId = "bbb867eb-f1e2-4deb-8a21-832f963b4a74";
    options.ClientSecret = "XIKHAcDO6oVYIAQQs8cewfaJwGxVV5u5x-6Yi-lu";
});

//����ʼ���������
builder.Services.AddScoped<IEmailSender, SmtpMailSender>()
    .Configure<SmtpMailSenderOptions>(builder.Configuration.GetSection("SmtpMailSenderOptions"));

builder.Services.AddScoped<IdApiService>();


//����ת������
builder.Services.AddScoped<IClaimsTransformation, ClaimTransformation>();

//Ŀ¼����
builder.Services.AddScoped<DirectoryServiceManager>()
    .AddScoped<IDirectoryServiceStore, DirectoryServiceStore>();
builder.Services.AddScoped<LogonAccountManager>()
    .AddScoped<ILogonAccountStore, LogonAccountStore>();

//��ȫ��ɫ����
builder.Services.AddScoped<UserInRoleManager>()
    .AddScoped<IUserInRoleStore, UserInRoleStore>();

//��Debugģʽʱ������ע����ǰ�����Խ���ⲿ����
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
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.MapRazorPages();
app.MapControllers();

//Run
app.Run();

namespace AdminWebApp
{
    public partial class Program { }
}