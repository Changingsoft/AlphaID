using AdminWebApp.Infrastructure;
using AlphaIDEntityFramework.EntityFramework;
using DatabaseTool;
using Microsoft.EntityFrameworkCore;


var builder = Host.CreateDefaultBuilder(args);
builder
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddDebug();
            logging.AddConsole();
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

var host = builder.Build();


PrepareDevelopmentData.EnsureDevelopmentData(host);

return;
