namespace RadiusCore;

/// <summary>
/// 连接请求的路由类型。
/// </summary>
public enum RouteType
{
    /// <summary>
    /// 由本地服务器处理。
    /// </summary>
    Local,
    /// <summary>
    /// 由远程服务器处理。
    /// </summary>
    Forward,
    /// <summary>
    /// 不验证就接受。
    /// </summary>
    Bypass,
}