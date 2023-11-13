using AdminWebApp.Infrastructure.DataStores;
using AlphaID.DirectoryLogon.EntityFramework;
using AlphaID.EntityFramework;
using DatabaseTool;
using DatabaseTool.Migrators;
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
            services.AddDbContext<IdSubjectsDbContext>(options =>
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

            //用于IdentityServer的ConfigurationDbContext和PersistenceGrantDbContext
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
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("AdminWebAppDataConnection"), sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            });

            //数据库执行器
            services.AddScoped<DatabaseExecutor>().Configure<DatabaseExecutorOptions>(options =>
            {
                options.DropDatabase = bool.Parse(hostContext.Configuration["DropDatabase"] ?? "false");
                options.AddTestingData = bool.Parse(hostContext.Configuration["AddTestingData"] ?? "false");
            });

            //配置迁移器
            services.AddScoped<DatabaseMigrator, IdServerConfigurationDbMigrator>();
            services.AddScoped<DatabaseMigrator, IdServerPersistedGrantDbMigrator>();
            services.AddScoped<DatabaseMigrator, IdSubjectsDbMigrator>();
            services.AddScoped<DatabaseMigrator, DirectoryLogonDbMigrator>();
            services.AddScoped<DatabaseMigrator, AdminCenterDbMigrator>();
        });

var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();
var executor = scope.ServiceProvider.GetRequiredService<DatabaseExecutor>();
await executor.ExecuteAsync();