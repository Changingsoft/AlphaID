namespace RadiusCore.Models;

/// <summary>
/// 提供对RADIUS客户端的存储能力。
/// </summary>
public interface IRadiusClientStore
{
    /// <summary>
    /// Get queryable collections of RADIUS Client.
    /// </summary>
    IQueryable<RadiusClient> RadiusClients { get; }

    /// <summary>
    /// Create RADIUS client.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    Task CreateAsync(RadiusClient client);

    /// <summary>
    /// Update RADIUS client.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    Task UpdateAsync(RadiusClient client);

    /// <summary>
    /// Delete RADIUS client.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    Task DeleteAsync(RadiusClient client);
}