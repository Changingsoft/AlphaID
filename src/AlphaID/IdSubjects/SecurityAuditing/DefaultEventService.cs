using IdSubjects.DependencyInjection;
using IdSubjects.SecurityAuditing.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 默认事件服务。
/// </summary>
public class DefaultEventService : IEventService
{
    /// <summary>
    /// The options
    /// </summary>
    protected IdSubjectsOptions Options { get; }

    /// <summary>
    /// The context
    /// </summary>
    protected IHttpContextAccessor Context { get; }

    /// <summary>
    /// The sink
    /// </summary>
    protected IEventSink Sink { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultEventService"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="context">The context.</param>
    /// <param name="sink">The sink.</param>
    public DefaultEventService(IOptions<IdSubjectsOptions> options, IHttpContextAccessor context, IEventSink sink)
    {
        this.Options = options.Value;
        this.Context = context;
        this.Sink = sink;
    }

    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">evt</exception>
    public async Task RaiseAsync(AuditLogEvent evt)
    {
        ArgumentNullException.ThrowIfNull(evt);

        if (this.CanRaiseEvent(evt))
        {
            await this.PrepareEventAsync(evt);
            await this.Sink.PersistAsync(evt);
        }
    }

    /// <summary>
    /// Indicates if the type of event will be persisted.
    /// </summary>
    /// <param name="evtType"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    public bool CanRaiseEventType(AuditLogEventTypes evtType)
    {
        return evtType switch
        {
            AuditLogEventTypes.Failure => this.Options.Events.RaiseFailureEvents,
            AuditLogEventTypes.Information => this.Options.Events.RaiseInformationEvents,
            AuditLogEventTypes.Success => this.Options.Events.RaiseSuccessEvents,
            AuditLogEventTypes.Error => this.Options.Events.RaiseErrorEvents,
            _ => throw new ArgumentOutOfRangeException(nameof(evtType)),
        };
    }

    /// <summary>
    /// Determines whether this event would be persisted.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns>
    ///   <c>true</c> if this event would be persisted; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanRaiseEvent(AuditLogEvent evt)
    {
        return this.CanRaiseEventType(evt.EventType);
    }

    /// <summary>
    /// Prepares the event.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns></returns>
    protected virtual async Task PrepareEventAsync(AuditLogEvent evt)
    {
        evt.ActivityId = this.Context.HttpContext?.TraceIdentifier;
        evt.TimeStamp = DateTime.UtcNow;
        evt.ProcessId = Environment.ProcessId;

        if (this.Context.HttpContext?.Connection.LocalIpAddress != null)
        {
            evt.LocalIpAddress = this.Context.HttpContext.Connection.LocalIpAddress.ToString() + ":" + this.Context.HttpContext.Connection.LocalPort;
        }
        else
        {
            evt.LocalIpAddress = "unknown";
        }

        evt.RemoteIpAddress = this.Context.HttpContext?.Connection.RemoteIpAddress != null ? this.Context.HttpContext.Connection.RemoteIpAddress.ToString() : "unknown";

        await evt.PrepareAsync();
    }
}
