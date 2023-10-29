using AdminWebApp.Infrastructure.DataStores;
using AlphaID.DirectoryLogon.EntityFramework;
using AlphaID.EntityFramework;
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
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("IDSubjectsDataConnection"), sql =>
                {
                    sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                    sql.UseNetTopologySuite();
                });
                options.UseLazyLoadingProxies();
            });

            services.AddDbContext<DirectoryLogonDbContext>(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("DirectoryLogonDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                options.UseLazyLoadingProxies();
            });

            //用于IdentityServer的ConfigurationDbContext和PersistanceGrantDbContext
            services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(hostContext.Configuration.GetConnectionString("OidcConfigurationDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(hostContext.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            });

            //用于AdminWebApp的OperationalDbContext
            services.AddDbContext<OperationalDbContext>(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("AdminWebAppDataConnection"), sql =>
                {
                    sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                });
            });
        });

var host = builder.Build();


PrepareDevelopmentData.EnsureDevelopmentData(host);

return;
