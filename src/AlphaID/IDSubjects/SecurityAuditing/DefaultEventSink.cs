using IdSubjects.SecurityAuditing.Events;
using Microsoft.Extensions.Logging;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="logger"></param>
public class DefaultEventSink(ILogger<DefaultEventService> logger) : IEventSink
{
    /// <summary>
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public Task PersistAsync(AuditLogEvent evt)
    {
        logger.LogInformation(evt.EventId, "{@event}", evt);
        return Task.CompletedTask;
    }
}