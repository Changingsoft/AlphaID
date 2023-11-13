using AlphaID.DirectoryLogon.EntityFramework;
using AlphaID.EntityFramework;
using AlphaIDWebAPI;
using AlphaIDWebAPI.Middlewares;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using IDSubjects.DirectoryLogon;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
    .WriteTo.EventLog(".NET Runtime", manageEventSource: true);
});

//Configuration Services
builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlInfo>(builder.Configuration.GetSection("SystemUrl"));
//启用Controller
builder.Services.AddControllers();
builder.Services.AddRazorPages();

//启用CORS
builder.Services.AddCors(options =>
{
    //添加默认策略。
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.SetIsOriginAllowed(_ => true)
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
builder.Services.AddDbContext<IdSubjectsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection"), sqlOptions =>
    {
        sqlOptions.UseNetTopologySuite();
    });
});
builder.Services.AddDbContext<DirectoryLogonDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DirectoryLogonDataConnection"));
});
builder.Services.AddDbContext<ConfigurationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OidcConfigurationDataConnection"));
}).AddScoped<ConfigurationStoreOptions>();


//builder.Services.AddScoped<INaturalPersonStore, NaturalPersonStore>();
builder.Services.AddScoped<IQueryableLogonAccountStore, QueryableLogonAccountStore>();

//ASP.NET Identity NaturalPersonManager.
builder.Services.AddIdSubjects(options =>
{

})
    .AddDefaultStores();


builder.Services.AddScoped<ChineseIdCardManager>()
    .AddScoped<IChineseIdCardValidationStore, RealNameValidationStore>();

var app = builder.Build();

//Configure Pipeline
app.UseSerilogRequestLogging();
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
    /// Definitions for Testing.
    /// </summary>
    public class Program { }
}