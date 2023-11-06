using AlphaID.EntityFramework;
using IDSubjects.ChineseName;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DataTool;

internal class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            })
            .ConfigureServices((hostContext, services) =>
            {
                //后台服务进程
                services.AddHostedService<PinyinCorrectionWork>();


                //数据库
                services.AddDbContext<IdSubjectsDbContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("AlphaIDDataConnection"));
                    options.UseLazyLoadingProxies();
                });

                //依赖项
                services.AddScoped<ChinesePersonNameFactory>();
                services.AddScoped<ChinesePersonNamePinyinConverter>();

            });
    }

    private static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.Message);
            throw;
        }
    }
}
