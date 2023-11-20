namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// 事件类型。
/// </summary>
public enum AuditLogEventTypes
{
    /// <summary>
    /// Success event
    /// </summary>
    Success = 1,

    /// <summary>
    /// Failure event
    /// </summary>
    Failure = 2,

    /// <summary>
    /// Information event
    /// </summary>
    Information = 3,

    /// <summary>
    /// Error event
    /// </summary>
    Error = 4
}