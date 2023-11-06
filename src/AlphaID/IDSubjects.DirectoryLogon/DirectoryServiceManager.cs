using Microsoft.Extensions.Logging;

namespace IDSubjects.DirectoryLogon;

/// <summary>
/// Directory service manager.
/// </summary>
public class DirectoryServiceManager
{
    private readonly IDirectoryServiceStore directoryServiceStore;
    private readonly ILogger<DirectoryServiceManager>? logger;

    /// <summary>
    /// Init DirectoryServiceManager.
    /// </summary>
    /// <param name="directoryServiceStore"></param>
    /// <param name="logger"></param>
    public DirectoryServiceManager(IDirectoryServiceStore directoryServiceStore, ILogger<DirectoryServiceManager>? logger = null)
    {
        this.directoryServiceStore = directoryServiceStore;
        this.logger = logger;
    }

    /// <summary>
    /// Gets list of DirectoryService.
    /// </summary>
    public IEnumerable<DirectoryService> Services => this.directoryServiceStore.Services;

    /// <summary>
    /// Create a directory service.
    /// </summary>
    /// <param name="directoryService"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public async Task<IdOperationResult> CreateAsync(DirectoryService directoryService)
    {
        using var entry = directoryService.GetRootEntry();
        try
        {
            var name = entry.Properties["name"].Value?.ToString();

            //没有异常，说明访问成功，可以持久化DirectoryService配置。
            await this.directoryServiceStore.CreateAsync(directoryService);
            return IdOperationResult.Success;
        }
        catch (Exception)
        {
            this.logger?.LogInformation("创建目录服务时出错，测试目录服务连接没有成功。");
            return IdOperationResult.Failed("创建目录服务时出错，测试目录服务连接没有成功。");
        }
        finally
        {
            entry.Dispose();
        }
    }

    /// <summary>
    /// Delete a directory service.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> DeleteAsync(DirectoryService data)
    {
        await this.directoryServiceStore.DeleteAsync(data);
        return IdOperationResult.Success;
    }

    /// <summary>
    /// Find Directory Service by Id.
    /// </summary>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    public Task<DirectoryService?> FindByIdAsync(int serviceId)
    {
        return this.directoryServiceStore.FindByIdAsync(serviceId);
    }
}
