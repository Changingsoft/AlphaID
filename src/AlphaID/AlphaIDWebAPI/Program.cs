using System.Reflection;
using AlphaId.EntityFramework;
using AlphaId.RealName.EntityFramework;
using AlphaIdPlatform;
using AlphaIdWebAPI;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using IdentityModel;
using IdSubjects.DependencyInjection;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .WriteTo.EventLog(".NET Runtime", restrictedToMinimumLevel: LogEventLevel.Information);
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
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["SwaggerOauthOptions:AuthorizationEndpoint"]!),
                TokenUrl = new Uri(builder.Configuration["SwaggerOauthOptions:TokenEndpoint"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "获取用户Id标识" },
                    { "profile", "获取用户基本信息" },
                    { "realname", "获取自然人的实名信息" }
                }
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
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    //添加JWT验证。
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = builder.Configuration["JwtOptions:MetadataAddress"]!;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

//添加授权策略。
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireClaim("scope", "openid")
        .RequireClaim(JwtClaimTypes.ClientId)
        .Build();

    options.AddPolicy("EndUser", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireClaim(JwtClaimTypes.Subject);
    });

    options.AddPolicy("RealNameScopeRequired", policyBuilder =>
    {
        policyBuilder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policyBuilder.RequireClaim("scope", "realname");
    });
});

//持久化
builder.Services.AddDbContext<ConfigurationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OidcConfigurationDataConnection"));
}).AddScoped<ConfigurationStoreOptions>();


IdSubjectsBuilder idSubjectsBuilder = builder.Services.AddIdSubjects();
idSubjectsBuilder
    .AddDefaultStores()
    .AddDbContext(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection"),
            sqlOptions => { sqlOptions.UseNetTopologySuite(); });
    });

if (bool.Parse(builder.Configuration[FeatureSwitch.RealNameFeature] ?? "false"))
    idSubjectsBuilder.AddRealName()
        .AddDefaultStores()
        .AddDbContext(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("IDSubjectsDataConnection")));


WebApplication app = builder.Build();

//Configure Pipeline
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger(options => { options.RouteTemplate = "docs/{documentName}/docs.json"; });
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/docs/v1/docs.json",
        $"{app.Configuration["OpenApiInfo:Title"]} {app.Configuration["OpenApiInfo:Version"]}");
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

namespace AlphaIdWebAPI
{
    /// <summary>
    ///     Definitions for Testing.
    /// </summary>
    public class Program;
}