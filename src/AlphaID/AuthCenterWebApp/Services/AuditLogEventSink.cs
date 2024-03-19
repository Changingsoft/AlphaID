using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;

namespace AuthCenterWebApp.Services;

/// <summary>
/// 替代 DuendeSoft.IdentityServer 的默认事件槽，在日志中增加记录EventId.
/// </summary>
public class AuditLogEventSink(ILogger<DefaultEventService> logger) : IEventSink
{
    public Task PersistAsync(Event evt)
    {
        logger.LogInformation(new EventId(evt.Id, evt.Name), "{@event}", evt);
        return Task.CompletedTask;
    }
}
