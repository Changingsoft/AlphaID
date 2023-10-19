using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace AuthCenterWebAppTests;

public class AuthCenterWebAppFactory : WebApplicationFactory<AuthCenterWebApp.Program>
{
    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        var server = base.CreateServer(builder);
        //if(!DatabaseReady)
        //{
        //    lock(_lock)
        //    {
        //        if(!DatabaseReady)
        //        {
        //            using var scope = server.Services.CreateAsyncScope();
        //            var idDb = scope.ServiceProvider.GetRequiredService<IDSubjectsDbContext>();
        //            idDb.Database.EnsureDeleted();
        //            idDb.Database.Migrate();
        //            scope.Dispose();
        //            DatabaseReady = true;
        //        }
        //    }
        //}
        return server;
    }


    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        //hack bypass database init process.
        return host;
#if DEBUG
        var workDir = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\DatabaseTool\bin\Debug\net7.0\";
        var process = new Process();
        process.StartInfo.FileName = workDir + @"DatabaseTool.exe";
        process.StartInfo.WorkingDirectory = workDir;
        process.Start();
        process.WaitForExit();
#endif
#if RELEASE
        var workDir = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\DatabaseTool\bin\Release\net7.0\";
        var process = new Process();
        process.StartInfo.FileName = workDir + @"DatabaseTool.exe";
        process.StartInfo.WorkingDirectory = workDir;
        process.Start();
        process.WaitForExit();
#endif


        return host;
    }
}
