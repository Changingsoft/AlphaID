using AdminWebApp.Infrastructure.DataStores;
using AlphaID.DirectoryLogon.EntityFramework;
using AlphaID.EntityFramework;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace DatabaseTool;

public class PrepareDevelopmentData
{
    public static void EnsureDevelopmentData(IHost app)
    {
        using var scope = app.Services.CreateScope();


        var idDbContext = scope.ServiceProvider.GetRequiredService<IDSubjectsDbContext>();
        var idSvrConfigurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        var idSvrOperationalDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        var directoryLogonDbContext = scope.ServiceProvider.GetRequiredService<DirectoryLogonDbContext>();
        var adminWebAppDbContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();

        //Apply Migrations
        idDbContext.Database.Migrate();
        idSvrConfigurationDbContext.Database.Migrate();
        idSvrOperationalDbContext.Database.Migrate();
        directoryLogonDbContext.Database.Migrate();
        adminWebAppDbContext.Database.Migrate();

        //Apply Example Data
        var idSubjectsDbSqlFiles = Directory.GetFiles("./TestingData/IDSubjectsDbContext", "*.sql");
        foreach (var file in idSubjectsDbSqlFiles)
        {
            idDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var configDbSqlFiles = Directory.GetFiles("./TestingData/ConfigurationDbContext", "*.sql");
        foreach (var file in configDbSqlFiles)
        {
            idSvrConfigurationDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var idserverOperationalSqlFiles = Directory.GetFiles("./TestingData/PersistedGrantDbContext", "*.sql");
        foreach (var file in idserverOperationalSqlFiles)
        {
            idSvrOperationalDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var directoryLogonDataSqlFiles = Directory.GetFiles("./TestingData/DirectoryLogonData", "*.sql");
        foreach (var file in directoryLogonDataSqlFiles)
        {
            directoryLogonDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
        var adminWebAppTestingFiles = Directory.GetFiles("./TestingData/AdminWebAppData", "*.sql");
        foreach(var file in adminWebAppTestingFiles)
        {
            adminWebAppDbContext.Database.ExecuteSqlRaw(File.ReadAllText(file, Encoding.UTF8));
        }
    }
}
