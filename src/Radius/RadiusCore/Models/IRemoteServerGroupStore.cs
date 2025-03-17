namespace RadiusCore.Models;

/// <summary>
/// 
/// </summary>
public interface IRemoteServerGroupStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<RemoteServerGroup> RemoteServerGroups { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    Task CreateAsync(RemoteServerGroup group);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    Task UpdateAsync(RemoteServerGroup group);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    Task DeleteAsync(RemoteServerGroup group);

}