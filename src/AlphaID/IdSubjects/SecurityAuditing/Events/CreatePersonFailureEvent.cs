using Microsoft.Extensions.Logging;

namespace IdSubjects.SecurityAuditing.Events;
internal class CreatePersonFailureEvent : AuditLogEvent
{
    public CreatePersonFailureEvent()
        : base(AuditLogEventCategories.AccountManagement, EventIds.CreatePersonFailure, AuditLogEventTypes.Failure)
    {
    }
}