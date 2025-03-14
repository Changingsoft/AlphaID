namespace RadiusCore.RadiusConstants;

/// <summary>
/// 
/// </summary>
public enum AcctTerminateCause
{
    /// <summary>
    /// 
    /// </summary>
    UserRequest = 1,
    /// <summary>
    /// 
    /// </summary>
    LostCarrier = 2,
    /// <summary>
    /// 
    /// </summary>
    LostService = 3,
    /// <summary>
    /// 
    /// </summary>
    IdleTimeout = 4,
    /// <summary>
    /// 
    /// </summary>
    SessionTimeout = 5,
    /// <summary>
    /// 
    /// </summary>
    AdminReset = 6,
    /// <summary>
    /// 
    /// </summary>
    AdminReboot = 7,
    /// <summary>
    /// 
    /// </summary>
    PortError = 8,
    /// <summary>
    /// 
    /// </summary>
    NASError = 9,
    /// <summary>
    /// 
    /// </summary>
    NASRequest = 10,
    /// <summary>
    /// 
    /// </summary>
    NASReboot = 11,
    /// <summary>
    /// 
    /// </summary>
    PortUnneeded = 12,
    /// <summary>
    /// 
    /// </summary>
    PortPreempted = 13,
    /// <summary>
    /// 
    /// </summary>
    PortSuspended = 14,
    /// <summary>
    /// 
    /// </summary>
    ServiceUnavailable = 15,
    /// <summary>
    /// 
    /// </summary>
    Callback = 16,
    /// <summary>
    /// 
    /// </summary>
    UserError = 17,
    /// <summary>
    /// 
    /// </summary>
    HostRequest = 18
}