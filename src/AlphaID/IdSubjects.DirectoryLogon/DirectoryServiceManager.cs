using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Directory service manager.
/// </summary>
/// <remarks>
/// Init DirectoryServiceManager.
/// </remarks>
/// <param name="directoryServiceStore"></param>
/// <param name="logger"></param>
public class DirectoryServiceManager(
    IDirectoryServiceStore directoryServiceStore,
    ILogger<DirectoryServiceManager>? logger = null)
{
    /// <summary>
    /// Gets list of DirectoryService.
    /// </summary>
    public IEnumerable<DirectoryService> Services => directoryServiceStore.Services;

    /// <summary>
    /// Create a directory service.
    /// </summary>
    /// <param name="directoryService"></param>
    public async Task<IdOperationResult> CreateAsync(DirectoryService directoryService)
    {
        if (!directoryService.DefaultUserAccountContainer.EndsWith(directoryService.RootDn))
            return IdOperationResult.Failed("默认UserContainer必须是RootDN的子集。");

        try
        {
            using PrincipalContext context = PrincipalContextHelper.GetRootContext(directoryService);

            //没有异常，说明访问成功，可以持久化DirectoryService配置。
            await directoryServiceStore.CreateAsync(directoryService);
            return IdOperationResult.Success;
        }
        catch (Exception)
        {
            logger?.LogInformation("创建目录服务时出错，测试目录服务连接没有成功。");
            return IdOperationResult.Failed("创建目录服务时出错，测试目录服务连接没有成功。");
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="directoryService"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> UpdateAsync(DirectoryService directoryService)
    {
        if (!directoryService.DefaultUserAccountContainer.EndsWith(directoryService.RootDn))
            return IdOperationResult.Failed("默认UserContainer必须是RootDN的子集。");

        try
        {
            using PrincipalContext context = PrincipalContextHelper.GetRootContext(directoryService);

            //没有异常，说明访问成功，可以持久化DirectoryService配置。
            await directoryServiceStore.UpdateAsync(directoryService);
            return IdOperationResult.Success;
        }
        catch (Exception)
        {
            logger?.LogInformation("创建目录服务时出错，测试目录服务连接没有成功。");
            return IdOperationResult.Failed("创建目录服务时出错，测试目录服务连接没有成功。");
        }
    }

    /// <summary>
    /// Delete a directory service.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> DeleteAsync(DirectoryService data)
    {
        await directoryServiceStore.DeleteAsync(data);
        return IdOperationResult.Success;
    }

    /// <summary>
    /// Search from directory service.
    /// </summary>
    /// <param name="directoryService"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public IEnumerable<DirectorySearchItem> Search(DirectoryService directoryService, string filter)
    {
        using DirectoryEntry searchRoot = DirectoryEntryHelper.GetRootEntry(directoryService);
        using DirectorySearcher searcher = new(searchRoot);
        searcher.Filter = filter;
        SearchResultCollection results = searcher.FindAll();
        HashSet<DirectorySearchItem> directorySearchItems = [];
        foreach (SearchResult searchResult in results)
        {
            using DirectoryEntry entry = searchResult.GetDirectoryEntry();
            directorySearchItems.Add(new DirectorySearchItem(entry.Properties["name"].Value!.ToString()!,
                entry.Properties["sAMAccountName"].Value?.ToString(),
                entry.Properties["userPrincipalName"].Value?.ToString()!,
                entry.Guid,
                entry.Properties["distinguishedName"].Value!.ToString()!,
                entry.Properties["displayName"].Value?.ToString(),
                entry.Properties["mobile"].Value?.ToString(),
                entry.Properties["company"].Value?.ToString(),
                entry.Properties["department"].Value?.ToString(),
                entry.Properties["title"].Value?.ToString()));
        }

        return directorySearchItems;
    }


    /// <summary>
    /// Find Directory Service by Id.
    /// </summary>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    public Task<DirectoryService?> FindByIdAsync(int serviceId)
    {
        return directoryServiceStore.FindByIdAsync(serviceId);
    }
}