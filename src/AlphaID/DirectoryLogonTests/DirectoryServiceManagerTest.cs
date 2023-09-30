namespace DirectoryLogonTests;

public class DirectoryServiceManagerTest
{
    [Fact(Skip = "���߱��ɲ�����")]
    public async void CreateDirectoryService()
    {
        var store = new StubDirectoryServiceStore();
        var manager = new DirectoryServiceManager(store);

        var directoryService = new DirectoryService()
        {
            ServerAddress = "localhost",
            RootDN = "DC=qjyc,DC=cn",
        };
        var result = await manager.CreateAsync(directoryService);
        Assert.True(result.IsSuccess);
    }
}