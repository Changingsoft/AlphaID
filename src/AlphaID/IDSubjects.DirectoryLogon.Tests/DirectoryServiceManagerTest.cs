using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.DirectoryLogon.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class DirectoryServiceManagerTest(ServiceProviderFixture serviceProvider)
{
    [Fact(Skip = "不具备可测试性")]
    public async void CreateDirectoryService()
    {
        using var scope = serviceProvider.ScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<DirectoryServiceManager>();

        var directoryService = new DirectoryServiceDescriptor()
        {
            ServerAddress = "localhost",
            RootDn = "DC=example,DC=com",
        };
        var result = await manager.CreateAsync(directoryService);
        Assert.True(result.Succeeded);
    }
}