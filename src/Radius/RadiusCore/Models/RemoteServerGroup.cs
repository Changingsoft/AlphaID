namespace RadiusCore.Models;

/// <summary>
/// 远程服务器组。
/// </summary>
public class RemoteServerGroup
{
    /// <summary>
    /// Id。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名称。
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 服务器列表。
    /// </summary>
    public virtual ICollection<RemoteServer> Servers { get; set; } = [];
}