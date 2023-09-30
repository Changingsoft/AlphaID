using AlphaIDEntityFramework.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OperationalEF;

namespace DatabaseTool;

internal class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddConsole();
            })
            .ConfigureAppConfiguration(config =>
            {
                //config.AddJsonFile("DatabaseToolSettings.json", true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<IDSubjectsDbContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("IDSubjectsDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                    options.UseLazyLoadingProxies();
                });
                services.AddDbContext<DirectoryLogonDbContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("DirectoryLogonDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                    options.UseLazyLoadingProxies();
                });
                services.AddDbContext<OperationalDbContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("OperationalDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                    options.UseLazyLoadingProxies();
                });
                services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(hostContext.Configuration.GetConnectionString("OidcConfigurationDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(hostContext.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                });
            });
    }

    private static void Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();

        PrepareDevelopmentData.EnsureDevelopmentData(host);

        //Console.WriteLine("操作已完成，按任意键继续");
        //Console.ReadKey(true);
        return;
    }
}
