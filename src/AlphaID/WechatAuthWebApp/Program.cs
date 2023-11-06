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

//����ע��HttpContext֧��
builder.Services.AddHttpContextAccessor();

//ע��EFCore DbContext
builder.Services.AddDbContext<IDSubjectsDbContext>(options =>
{
    //ʹ��SQL Server���ݿ⡣
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //ʹ�ö�̬������������ء�
    options.UseLazyLoadingProxies();
});
//ע��EFCore DbContext
builder.Services.AddDbContext<WechatWebLoginDbContext>(options =>
{
    //ʹ��SQL Server���ݿ⡣
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //ʹ�ö�̬������������ء�
    options.UseLazyLoadingProxies();
});

//ע��ҵ��
builder.Services.AddScoped<OAuth2Service>();
builder.Services.AddScoped<NaturalPersonManager>();
builder.Services.AddScoped<WechatLoginSessionManager>();
builder.Services.AddScoped<IWechatLoginSessionStore, WechatLoginSessionStore>();
builder.Services.AddScoped<IWechatServiceProvider, WechatServiceProvider>();
builder.Services.AddScoped<IWechatAppClientStore, WechatSPAConfidentialClientStore>();
builder.Services.AddScoped<IVerificationCodeService, SimpleShortMessageService>();
builder.Services.AddScoped<IChineseIDCardOCRService, AliyunChineseIDCardOCRService>();

//�־û�
//builder.Services.AddScoped<INaturalPersonStore, NaturalPersonStore>();
builder.Services.AddScoped<IQueryableLogonAccountStore, QueryableLogonAccountStore>();
builder.Services.AddScoped<IWechatUserIdentifierStore, WechatUserIdentifierStore>();

//����
builder.Services.Configure<SimpleShortMessageServiceOptions>(builder.Configuration.GetSection("SimpleShortMessageServiceOptions"));

#if DEBUG
//���������ڵ��ԡ�
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