using AlphaIDEntityFramework.EntityFramework;
using AlphaIDWebAPI;
using AlphaIDWebAPI.Middlewares;
using DirectoryLogon;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Configuration Services
builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlInfo>(builder.Configuration.GetSection("SystemUrl"));
//启用Controller
builder.Services.AddControllers(o =>
{

});
builder.Services.AddRazorPages();

//启用CORS
builder.Services.AddCors(options =>
{
    //添加默认策略。
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader()
        .SetPreflightMaxAge(TimeSpan.FromDays(20));
    });

});

builder.Services.AddSwaggerGen(options =>
{
    var info = builder.Configuration.GetSection("OpenApiInfo").Get<OpenApiInfo>();
    options.SwaggerDoc("v1", info);
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
    options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            AuthorizationCode = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri(builder.Configuration["SwaggerOauthOptions:AuthorizationEndpoint"]!),
                TokenUrl = new Uri(builder.Configuration["SwaggerOauthOptions:TokenEndpoint"]!),
                Scopes = new Dictionary<string, string>()
                        {
                            {"openid", "获取用户Id标识" },
                            {"profile", "获取用户基本信息" },
                            {"realname", "获取自然人的实名信息" },
                        },
            }
        }
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//配置身份验证服务。
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    //添加JWT验证。
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = builder.Configuration["JwtOptions:MetadataAddress"]!;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
        };
    })
    .AddOpenIdConnect(options =>
    {
        options.MetadataAddress = builder.Configuration["JwtOptions:MetadataAddress"];
        options.ClientId = builder.Configuration["JwtOptions:ClientId"];
        options.ClientSecret = builder.Configuration["JwtOptions:ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
    })
    .AddCookie();
//builder.Services.Configure<JwtBearerOptions>(builder.Configuration.GetSection("JwtBearerOptions"));

//添加授权策略。
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
    .RequireClaim("scope", "openid")
    .Build();

    options.AddPolicy("RealNameScopeRequired", builder =>
    {
        builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        builder.RequireClaim("scope", "realname");
    });

});

//持久化
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


//builder.Services.AddScoped<INaturalPersonStore, NaturalPersonStore>();
builder.Services.AddScoped<IOrganizationStore, OrganizationStore>();
builder.Services.AddScoped<IQueryableOrganizationStore, OrganizationStore>();
builder.Services.AddScoped<IQueryableLogonAccountStore, QueryableLogonAccountStore>();

//ASP.NET Identity NaturalPersonManager.
builder.Services.AddIdentityCore<NaturalPerson>()
    .AddUserManager<NaturalPersonManager>()
    .AddUserStore<NaturalPersonStore>();

//组织成员
builder.Services.AddScoped<OrganizationMemberManager>()
    .AddScoped<IOrganizationMemberStore, OrganizationMemberStore>();

builder.Services.AddScoped<ChineseIDCardManager>()
    .AddScoped<IChineseIDCardValidationStore, RealNameValidationStore>();

var app = builder.Build();

//Configure Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
if (bool.Parse(builder.Configuration["ProtectSwagger"]!))
    app.UseSwaggerAuthorized();
app.UseSwagger(options =>
{
    options.RouteTemplate = "docs/{documentName}/docs.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/docs/v1/docs.json", $"{app.Configuration["OpenApiInfo:Title"]} {app.Configuration["OpenApiInfo:Version"]}");
    options.DocumentTitle = app.Configuration["OpenApiInfo:Title"];
    options.RoutePrefix = "docs";
    options.InjectStylesheet("/swagger-ui/custom.css");
    options.OAuthScopes("openid", "profile");
    options.OAuthUsePkce();
});
app.MapRazorPages();
app.MapControllers();

//Run
app.Run();

namespace AlphaIDWebAPI
{
    /// <summary>
    /// Definations for Testing.
    /// </summary>
    public partial class Program { }
}