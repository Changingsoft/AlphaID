using AdminWebApp.Infrastructure.DataStores;
using AlphaId.DirectoryLogon.EntityFramework;
using AlphaId.EntityFramework;
using AlphaId.EntityFramework.SecurityAuditing;
using AlphaId.RealName.EntityFramework;
using DatabaseTool;
using DatabaseTool.Migrators;
using IdSubjects.DependencyInjection;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using IdSubjects.SecurityAuditing;
using Microsoft.EntityFrameworkCore;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddDebug();
        logging.AddConsole();
    })
    .ConfigureServices((hostContext, services) =>
    {
        IdSubjectsBuilder idSubjectsBuilder = services.AddIdSubjects()
            .AddDefaultStores()
            .AddDbContext(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("IDSubjectsDataConnection"), sql =>
                {
                    sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                    sql.UseNetTopologySuite();
                });
            });

        idSubjectsBuilder.AddRealName()
            .AddDefaultStores()
            .AddDbContext(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("IDSubjectsDataConnection"),
                    sql => { sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name); });
            });

        idSubjectsBuilder.AddDirectoryLogin()
            .AddDefaultStores()
            .AddDbContext(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("DirectoryLogonDataConnection"),
                    sql => { sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name); });
            });


        //用于IdentityServer的ConfigurationDbContext和PersistenceGrantDbContext
        services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(hostContext.Configuration.GetConnectionString("OidcConfigurationDataConnection"),
                        sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(
                        hostContext.Configuration.GetConnectionString("OidcPersistedGrantDataConnection"),
                        sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            });

        //用于AdminWebApp的OperationalDbContext
        services.AddDbContext<OperationalDbContext>(options =>
        {
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("AdminWebAppDataConnection"),
                sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
        });

        services.AddAuditLog()
            .AddDefaultStore()
            .AddDbContext(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("IDSubjectsDataConnection"),
                    sql => { sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name); });
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
        services.AddScoped<DatabaseMigrator, RealNameDbMigrator>();
        services.AddScoped<DatabaseMigrator, DirectoryLogonDbMigrator>();
        services.AddScoped<DatabaseMigrator, AdminCenterDbMigrator>();
        services.AddScoped<DatabaseMigrator, AuditLogDbMigrator>();
    });

IHost host = builder.Build();

await using AsyncServiceScope scope = host.Services.CreateAsyncScope();
var executor = scope.ServiceProvider.GetRequiredService<DatabaseExecutor>();
await executor.ExecuteAsync();