using AlphaID.DirectoryLogon.EntityFramework;
using AlphaID.EntityFramework;
using AlphaID.PlatformServices.Aliyun;
using AlphaID.PlatformServices.Primitives;
using AlphaID.WechatWebLogin.EntityFramework;
using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using DirectoryLogon;
using IDSubjects;
using Microsoft.EntityFrameworkCore;
using WechatWebLogin;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProductInfo>(builder.Configuration.GetSection("ProductInfo"));
builder.Services.Configure<SystemUrlInfo>(builder.Configuration.GetSection("SystemUrl"));
builder.Services.AddRazorPages();

//启用注入HttpContext支持
builder.Services.AddHttpContextAccessor();

//注册EFCore DbContext
builder.Services.AddDbContext<IDSubjectsDbContext>(options =>
{
    //使用SQL Server数据库。
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //使用动态代理进行懒加载。
    options.UseLazyLoadingProxies();
});
//注册EFCore DbContext
builder.Services.AddDbContext<WechatWebLoginDbContext>(options =>
{
    //使用SQL Server数据库。
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //使用动态代理进行懒加载。
    options.UseLazyLoadingProxies();
});

//注册业务
builder.Services.AddScoped<OAuth2Service>();
builder.Services.AddScoped<NaturalPersonManager>();
builder.Services.AddScoped<WechatLoginSessionManager>();
builder.Services.AddScoped<IWechatLoginSessionStore, WechatLoginSessionStore>();
builder.Services.AddScoped<IWechatServiceProvider, WechatServiceProvider>();
builder.Services.AddScoped<IWechatAppClientStore, WechatSPAConfidentialClientStore>();
builder.Services.AddScoped<IVerificationCodeService, SimpleShortMessageService>();
builder.Services.AddScoped<IChineseIDCardOCRService, AliyunChineseIDCardOCRService>();

//持久化
//builder.Services.AddScoped<INaturalPersonStore, NaturalPersonStore>();
builder.Services.AddScoped<IQueryableLogonAccountStore, QueryableLogonAccountStore>();
builder.Services.AddScoped<IWechatUserIdentifierStore, WechatUserIdentifierStore>();

//配置
builder.Services.Configure<SimpleShortMessageServiceOptions>(builder.Configuration.GetSection("SimpleShortMessageServiceOptions"));

#if DEBUG
//覆盖以用于调试。
builder.Services.AddScoped<IChineseIDCardOCRService, DebugIDCardOCRService>();
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");

}

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHttpsRedirection();
app.UseHsts();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

app.Run();

namespace WechatAuthWebApp
{
    public partial class Program { }
}