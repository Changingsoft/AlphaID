using IdSubjects.SecurityAuditing.Events;
using Microsoft.Extensions.Options;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 默认事件服务。
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultEventService" /> class.
/// </remarks>
/// <param name="options">The options.</param>
/// <param name="sink">The sink.</param>
public class DefaultEventService(IOptions<AuditEventsOptions> options, IEventSink sink)
    : IEventService
{
    /// <summary>
    /// The options
    /// </summary>
    protected AuditEventsOptions Options { get; } = options.Value;

    /// <summary>
    /// The sink
    /// </summary>
    protected IEventSink Sink { get; } = sink;

    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">evt</exception>
    public virtual async Task RaiseAsync(AuditLogEvent evt)
    {
        ArgumentNullException.ThrowIfNull(evt);

        if (CanRaiseEvent(evt))
        {
            await PrepareEventAsync(evt);
            await Sink.PersistAsync(evt);
        }
    }

    /// <summary>
    /// Indicates if the type of event will be persisted.
    /// </summary>
    /// <param name="evtType"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    public virtual bool CanRaiseEventType(AuditLogEventTypes evtType)
    {
        return evtType switch
        {
            AuditLogEventTypes.Failure => Options.RaiseFailureEvents,
            AuditLogEventTypes.Information => Options.RaiseInformationEvents,
            AuditLogEventTypes.Success => Options.RaiseSuccessEvents,
            AuditLogEventTypes.Error => Options.RaiseErrorEvents,
            _ => throw new ArgumentOutOfRangeException(nameof(evtType))
        };
    }

    /// <summary>
    /// Determines whether this event would be persisted.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns>
    /// <c>true</c> if this event would be persisted; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanRaiseEvent(AuditLogEvent evt)
    {
        return CanRaiseEventType(evt.EventType);
    }

    /// <summary>
    /// Prepares the event.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns></returns>
    protected virtual async Task PrepareEventAsync(AuditLogEvent evt)
    {
        evt.TimeStamp = DateTime.UtcNow;
        evt.ProcessId = Environment.ProcessId;

        // 让事件自己做准备。
        await evt.PrepareAsync();
    }
}