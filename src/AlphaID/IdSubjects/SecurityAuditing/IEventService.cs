using IdSubjects.SecurityAuditing.Events;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// Interface for the event service.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <param name="evt">The event.</param>
    Task RaiseAsync(AuditLogEvent evt);

    /// <summary>
    /// Indicates if the type of event will be persisted.
    /// </summary>
    bool CanRaiseEventType(AuditLogEventTypes evtType);
}
