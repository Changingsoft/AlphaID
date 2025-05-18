using IdSubjects.SecurityAuditing.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 默认事件服务。
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultEventService" /> class.
/// </remarks>
/// <param name="options">The options.</param>
/// <param name="context">The context.</param>
/// <param name="sink">The sink.</param>
public class WebAppEventService(IOptions<AuditEventsOptions> options, IHttpContextAccessor context, IEventSink sink)
    : DefaultEventService(options, sink)
{
    /// <summary>
    /// The context
    /// </summary>
    protected IHttpContextAccessor Context { get; } = context;

    /// <summary>
    /// Prepares the event.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns></returns>
    protected override async Task PrepareEventAsync(AuditLogEvent evt)
    {
        await base.PrepareEventAsync(evt);
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
    }
}