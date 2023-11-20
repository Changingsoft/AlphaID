using IdSubjects.SecurityAuditing.Events;
using Microsoft.Extensions.Logging;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 
/// </summary>
public class DefaultEventSink : IEventSink
{
    private readonly ILogger<DefaultEventService> logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public DefaultEventSink(ILogger<DefaultEventService> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public Task PersistAsync(AuditLogEvent evt)
    {
        this.logger.LogInformation(evt.EventId, "{@event}", evt);
        return Task.CompletedTask;
    }
}
