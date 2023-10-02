using AlphaIDEntityFramework.EntityFramework;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using OperationalEF;
using System.Text;

namespace DatabaseTool;

public class PrepareDevelopmentData
{
    public static void EnsureDevelopmentData(IHost app)
    {
        using var scope = app.Services.CreateScope();


        var idDbContext = scope.ServiceProvider.GetRequiredService<IDSubjectsDbContext>();
        var adminCenterDbContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
        var idSvrConfigurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        var idSvrOperationalDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        var directoryLogonDbContext = scope.ServiceProvider.GetRequiredService<DirectoryLogonDbContext>();

        //if any database exists, delete it first.
        idDbContext.Database.EnsureDeleted();
        adminCenterDbContext.Database.EnsureDeleted();
        idSvrConfigurationDbContext.Database.EnsureDeleted();
        idSvrOperationalDbContext.Database.EnsureDeleted();
        directoryLogonDbContext.Database.EnsureDeleted();


        //Apply Migrations
        idDbContext.Database.Migrate();
        adminCenterDbContext.Database.Migrate();
        idSvrConfigurationDbContext.Database.Migrate();
        idSvrOperationalDbContext.Database.Migrate();
        directoryLogonDbContext.Database.Migrate();

        //Apply Example Data
        var idSubjectsDbSqlFiles = Directory.GetFiles("./sql/IDSubjectsDbContext", "*.sql");
        foreach (var file in idSubjectsDbSqlFiles)
        {
            idDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var configDbSqlFiles = Directory.GetFiles("./sql/ConfigurationDbContext", "*.sql");
        foreach (var file in configDbSqlFiles)
        {
            idSvrConfigurationDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var idserverOperationalSqlFiles = Directory.GetFiles("./sql/PersistedGrantDbContext", "*.sql");
        foreach (var file in idserverOperationalSqlFiles)
        {
            idSvrOperationalDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var appSecuritySqlFiles = Directory.GetFiles("./sql/AdminCenterDbContext", "*.sql");
        foreach (var file in appSecuritySqlFiles)
        {
            adminCenterDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var directoryLogonDataSqlFiles = Directory.GetFiles("./sql/DirectoryLogonData", "*.sql");
        foreach (var file in directoryLogonDataSqlFiles)
        {
            directoryLogonDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
    }
}
