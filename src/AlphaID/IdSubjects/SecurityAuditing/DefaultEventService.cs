using IdSubjects.SecurityAuditing.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdSubjects.SecurityAuditing;

/// <summary>
///     默认事件服务。
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="DefaultEventService" /> class.
/// </remarks>
/// <param name="options">The options.</param>
/// <param name="context">The context.</param>
/// <param name="sink">The sink.</param>
public class DefaultEventService(IOptions<AuditEventsOptions> options, IHttpContextAccessor context, IEventSink sink)
    : IEventService
{
    /// <summary>
    ///     The options
    /// </summary>
    protected AuditEventsOptions Options { get; } = options.Value;

    /// <summary>
    ///     The context
    /// </summary>
    protected IHttpContextAccessor Context { get; } = context;

    /// <summary>
    ///     The sink
    /// </summary>
    protected IEventSink Sink { get; } = sink;

    /// <summary>
    ///     Raises the specified event.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">evt</exception>
    public async Task RaiseAsync(AuditLogEvent evt)
    {
        ArgumentNullException.ThrowIfNull(evt);

        if (CanRaiseEvent(evt))
        {
            await PrepareEventAsync(evt);
            await Sink.PersistAsync(evt);
        }
    }

    /// <summary>
    ///     Indicates if the type of event will be persisted.
    /// </summary>
    /// <param name="evtType"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    public bool CanRaiseEventType(AuditLogEventTypes evtType)
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
    ///     Determines whether this event would be persisted.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns>
    ///     <c>true</c> if this event would be persisted; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanRaiseEvent(AuditLogEvent evt)
    {
        return CanRaiseEventType(evt.EventType);
    }

    /// <summary>
    ///     Prepares the event.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns></returns>
    protected virtual Task PrepareEventAsync(AuditLogEvent evt)
    {
        evt.ActivityId = Context.HttpContext?.TraceIdentifier;
        evt.TimeStamp = DateTime.UtcNow;
        evt.ProcessId = Environment.ProcessId;

        if (Context.HttpContext?.Connection.LocalIpAddress != null)
            evt.LocalIpAddress = Context.HttpContext.Connection.LocalIpAddress + ":" +
                                 Context.HttpContext.Connection.LocalPort;
        else
            evt.LocalIpAddress = "unknown";

        evt.RemoteIpAddress = Context.HttpContext?.Connection.RemoteIpAddress != null
            ? Context.HttpContext.Connection.RemoteIpAddress.ToString()
            : "unknown";

        // 让事件自己做准备。
        return evt.PrepareAsync();
    }
}