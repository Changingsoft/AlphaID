using AlphaId.InitData;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.Extensions.Options;

namespace DatabaseTool.Migrators;

internal class IdServerConfigurationDbMigrator(
    ConfigurationDbContext db,
    IOptions<DatabaseExecutorOptions> options,
    ILogger<IdServerConfigurationDbMigrator> logger)
    : DatabaseMigrator(db)
{
    public override async Task AddInitDataAsync()
    {
        InitDataResult result = await InitDataSeeder.EnsureAsync(db, new InitDataEnsureOptions
        {
            OverwriteExisting = options.Value.OverwriteInitData,
        });

        if (result.Inserted > 0)
            logger.LogInformation("已补齐 {Count} 项 IdentityServer 内置数据。", result.Inserted);
        else
            logger.LogInformation("IdentityServer 内置数据已存在，无需写入。");

        foreach (string warning in result.Warnings)
            logger.LogWarning("{Warning}", warning);
    }
}